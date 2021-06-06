using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PenedaVes.Models;
using PenedaVes.ViewModels;

namespace PenedaVes.Data.Repository
{ //        Startup.cs -> ConfigureServices->   services.AddTransient<IRepository, Repository>();
    public interface IRepository
    {
        /* Returns the Cameras a user follows */
        Task<List<Camera>> GetFollowedCameras(ApplicationUser user);
        /* Returns the Species a user follows */
        Task<List<Species>> GetFollowedSpecies(ApplicationUser user);

        /* Returns the Sightings captured in the last 7 days be the followed cameras and that contain
         the followed species by the user*/
        Task<List<Sighting>> GetFollowedSightings(List<Camera> followedCameras, List<Species> followedSpecies);
        
        /* Returns info about the cameras of the application, such as number of sightings in the last 7 days */
        List<CameraInfo> GetCameraInfo(List<Camera> followedCameras, List<Species> followedSpecies);
        
        /* Returns the sightings captures by a camera in a time limit, subject to the species the user follows */
        Task<List<Sighting>> GetCameraSightings(Camera camera, ApplicationUser user, DateTime lowerLimit, DateTime upperLimit);
        
        /* Returns the sightings involving a species in a time limit, subject to the cameras the user follows */
        Task<List<Sighting>> GetSpeciesSightings(Species species, ApplicationUser user, DateTime lowerLimit, DateTime upperLimit);
    }
}