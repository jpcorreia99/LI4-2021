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
            
            DateTime sevenDaysAgo = DateTime.Today.AddDays(-7);
            
            return await (from sighting in _context.Sightings
                    where sighting.CaptureMoment > sevenDaysAgo &&
                          followedCameras.Contains(sighting.Camera) && // select only sightings in the user's camera preferences
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
                          followedCamerasIds.Contains(sighting.SpeciesId)
                    select sighting)
                .Include(s => s.Camera)
                .OrderByDescending(x => x.CaptureMoment)
                .ToListAsync();
        }
    }
}