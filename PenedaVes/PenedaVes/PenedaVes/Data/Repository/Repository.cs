using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PenedaVes.Models;
using PenedaVes.ViewModels;

namespace PenedaVes.Data.Repository
{
    public class Repository : IRepository
    {
        private AppDbContext _context;
        private LinkGenerator _linkGenerator;

        public Repository(AppDbContext context,
            LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        private async Task<List<Camera>> GetFollowedCameras(ApplicationUser user)
        {
            return await _context.FollowedCamera
                .Include(fc => fc.Camera)
                .Where(fc => fc.UserId.Equals(user.Id))
                .Select(fc => fc.Camera)
                .ToListAsync();
        }
        
        private async Task<List<Species>> GetFollowedSpecies(ApplicationUser user)
        {
            return await _context.FollowedSpecies
                .Include(fs => fs.Species)
                .Where(fs => fs.UserId.Equals(user.Id))
                .Select(fs => fs.Species)
                .ToListAsync();

        }

        public async Task<List<Sighting>> GetFollowedSightings(ApplicationUser user, DateTime lowerLimit, DateTime upperLimit)
        {
            List<Camera> followedCameras = await GetFollowedCameras(user);
            List<Species> followedSpecies = await GetFollowedSpecies(user);

            return await (from sighting in _context.Sightings
                    where followedCameras.Contains(sighting.Camera) && // select only sightings in the user's camera preferences
                          followedSpecies.Contains(sighting.Species) && // select only sightings in the user's species preferences
                          sighting.CaptureMoment > lowerLimit &&
                          sighting.CaptureMoment <= upperLimit
                    select sighting)
                .Include(s => s.Camera)
                .Include(s => s.Species)
                .OrderByDescending(x => x.CaptureMoment).ToListAsync();
        }

        public async Task<List<CameraInfo>> GetCameraInfo(ApplicationUser user)
        {
            List<Camera> followedCameras = await GetFollowedCameras(user);
            List<Species> followedSpecies = await GetFollowedSpecies(user);
            DateTime sevenDaysAgo = DateTime.Today.AddDays(-7);
            
            return (from camera in followedCameras
                let sightingCount = (
                    from sighting in _context.Sightings
                    where sighting.CameraId == camera.Id &&
                          sighting.CaptureMoment > sevenDaysAgo &&
                          followedCameras.Contains(sighting.Camera) && // select only sightings in the user's camera preferences
                          followedSpecies.Contains(sighting.Species) // select only sightings in the user's species preferences
                    select sighting).Count() 
                select new CameraInfo(camera.Name, camera.Latitude, camera.Longitude, camera.RestrictedZone,
                    _linkGenerator.GetPathByAction("Details", "Cameras", new {id = camera.Id}),
                    sightingCount)).ToList();
        }

        public async Task<List<Sighting>> GetCameraSightings(Camera camera, ApplicationUser user, DateTime lowerLimit, DateTime upperLimit)
        {
            List<int> followedSpeciesIds = (await GetFollowedSpecies(user))
                                            .Select(s => s.Id)
                                            .ToList();

            return await (from sighting in _context.Sightings
                           where sighting.CameraId == camera.Id && 
                                    sighting.CaptureMoment > lowerLimit &&
                                    sighting.CaptureMoment <= upperLimit &&
                                    followedSpeciesIds.Contains(sighting.SpeciesId)
                           select sighting)
                        .Include(s => s.Species)
                        .OrderByDescending(x => x.CaptureMoment)
                        .ToListAsync();
        }

        public async Task<List<Sighting>> GetSpeciesSightings(Species species, ApplicationUser user,
            DateTime lowerLimit, DateTime upperLimit)
        {
            List<int> followedCamerasIds = (await GetFollowedCameras(user))
                .Select(s => s.Id)
                .ToList();

            return await (from sighting in _context.Sightings
                    where sighting.SpeciesId == species.Id && 
                          sighting.CaptureMoment > lowerLimit &&
                          sighting.CaptureMoment <= upperLimit &&
                          followedCamerasIds.Contains(sighting.CameraId)
                    select sighting)
                .Include(s => s.Camera)
                .OrderByDescending(x => x.CaptureMoment)
                .ToListAsync();
        }
        
        public BarChart GetBarChart(List<Sighting> sightings, DateTime lowerLimit, DateTime upperLimit, bool isSpecies)
        {
            List<DateTime> dateList = GetDays(lowerLimit, upperLimit);

            Dictionary<DateTime, int> sightingsCountByDate = new Dictionary<DateTime, int>();

            // This cycle will iterate by all the years, months and days where a sighting has been captured
            // It eill then group them and insert their count in the sightingsCountByDate dict
            foreach (var sightingsInYear in 
                sightings.GroupBy(s => s.CaptureMoment.Year)) // iterate by all the years
            {
                foreach (var sightingsInMonth in 
                    sightingsInYear.GroupBy(s => s.CaptureMoment.Month)) // iterate by all the months
                {
                    foreach (var sightingsInDay in 
                        sightingsInMonth.GroupBy(s => s.CaptureMoment.Day)) // iterate by all the days
                    {
                        // (year,month,day) of grouped sightings as dict key
                        // if it's species the barchart contains the number of animals seen, if it's a camera
                        // it's just the number of sightings
                        sightingsCountByDate[ new DateTime(sightingsInYear.Key, sightingsInMonth.Key,  
                                    sightingsInDay.Key)] = 
                            isSpecies ? sightingsInDay.Sum(sighting => sighting.Quantity) : sightingsInDay.Count();
                    }
                } 
            }

            List<int> dailySightings = new List<int>(); // holds the number of sightings in a day
            foreach (DateTime date in dateList)
            {
                DateTime key = date.Date;
                dailySightings.Add(sightingsCountByDate.ContainsKey(key) ? sightingsCountByDate[key] : 0);
            }

            string label = isSpecies ? "Animais avistados" : "Avistamentos capturados";
            List<String> dateListString = dateList.Select(day => day.ToString("dd/MM")).ToList();
            BarChartChild barChartChild = new BarChartChild
            {
                Label = label,
                BackgroundColor = "rgba(33, 150, 234, 0.62)",
                BorderColor = "rgba(33, 150, 234, 1)",
                BorderWidth = 2,
                HoverBackgroundColor = "rgba(33, 150, 234, 1)",
                HoverBorderColor = "rgba(33, 150, 234, 1)",
                Data = dailySightings
            };
            
            List<BarChartChild> datasets = new List<BarChartChild> {barChartChild};

            return new BarChart
            {
                Labels = dateListString,
                Datasets = datasets
            };
        }

        // Returns list of days between the given bounds
        private List<DateTime> GetDays(DateTime lowerLimit, DateTime upperLimit)
        {
            List<DateTime> dateList = new List<DateTime>();
            
            while (lowerLimit.AddDays(1) <= upperLimit)
            {
                lowerLimit = lowerLimit.AddDays(1);
                dateList.Add(lowerLimit);
            }

            return dateList;
        }
    }
}