using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PenedaVes.Models;
using PenedaVes.ViewModels;

namespace PenedaVes.Data.Repository
{ 
    public interface IRepository
    {

        /* Returns the Sightings captured in the the followed cameras and that contain
         the followed species by the user, defined by a time interval*/
        Task<List<Sighting>> GetFollowedSightings(ApplicationUser user, DateTime lowerLimit, DateTime upperLimit );
        
        /* Returns info about the cameras of the application, such as number of sightings in the last 7 days */
        Task<List<CameraInfo>> GetCameraInfo(ApplicationUser user);
        
        /* Returns the sightings captures by a camera in a time limit, subject to the species the user follows */
        Task<List<Sighting>> GetCameraSightings(Camera camera, ApplicationUser user, DateTime lowerLimit, DateTime upperLimit);
        
        /* Returns the sightings involving a species in a time limit, subject to the cameras the user follows */
        Task<List<Sighting>> GetSpeciesSightings(Species species, ApplicationUser user, DateTime lowerLimit, DateTime upperLimit);

        /* Creates a Bar Chart based on the time distribuition of the given sightings.
        If it's species the barchart contains the number of animals seen, if it's a camera it's just the number of sightings*/
        BarChart GetBarChart(List<Sighting> sightings, DateTime lowerLimit, DateTime upperLimit, bool isSpecies);
    }
}