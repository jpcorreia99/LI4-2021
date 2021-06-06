using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using PenedaVes.Data;

namespace PenedaVes.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<AppDbContext>>()))
            {
                // Check if DB is empty
                if (context.Species.Any())
                {
                    return;   // DB has been seeded
                }

                Species lobo = new Species
                {
                    CommonName = "Lobo Ibérico",
                    ScientificName = "Canis lupus signatus",
                    Description =
                        "O lobo-ibérico (Canis lupus signatus) é uma subespécie do lobo-cinzento que ocorre na Península Ibérica. Outrora muito abundante, a sua população atual deve rondar os 2000 indivíduos, dos quais cerca de 300 habitam o Norte de Portugal.",
                    IsPredatory = true,
                    Image = "img_05-06-2021-17-42-012.jpg"
                };
                
                Species corco = new Species
                {
                    CommonName = "Corço",
                    ScientificName = "Capreolus capreolus",
                    Description =
                        "O corço europeu, o menor dos cervídeos europeus, possui um aspecto gracioso e esbelto. Os seus membros posteriores são mais alargados e elevados que os anteriores, sendo especialmente adaptados ao salto. ",
                    IsPredatory = false,
                    Image = "img_05-06-2021-17-47-362.jpg"
                };


                Species vibora = new Species
                {
                    CommonName = "Víbora Cornuda",
                    ScientificName = "Vipera latastei",
                    Description =
                        "A víbora-cornuda (Vipera latastei) é uma espécie de víbora venenosa endémica da Península Ibérica e do noroeste da África. São actualmente reconhecidas duas sub-espécies incluindo a sub-espécie nominal aqui descrita",
                    IsPredatory = true,
                    Image = "img_05-06-2021-17-53-392.jpg"
                };

                Species garrano = new Species
                {
                    CommonName = "Garrano",
                    ScientificName = "Equus ferus caballus",
                    Description =
                        "A raça Garrana é uma raça ibérica autóctone de equídeos. O Garrano, palavra provável de origem proto-celta gearran, é uma raça de equideo muito antiga, separada das restantes desde o período Quaternário, que se enquadra num grupo alargado conhecido por Cavalo Ibérico devido às características comuns e à sua origem.",
                    IsPredatory = false,
                    Image = "img_05-06-2021-17-58-482.jpg"
                };
                
                Species tartanhao = new Species
                {
                    CommonName = "Tartaranhão-caçador",
                    ScientificName = "Circus pygargus",
                    Description =
                        "O tartaranhão-caçador ou águia-caçadeira (Circus pygargus) é uma ave pertencente à família Accipitridae.",
                    IsPredatory = false,
                    Image = "img_05-06-2021-18-13-422.jpg"
                };
                
                Species humano = new Species
                {
                    CommonName = "Humano",
                    ScientificName = "Homo sapiens",
                    Description =
                        "Visitantes do parque - devem evitar aceder a zonas de acesso restrito",
                    IsPredatory = false,
                    Image = "img_05-06-2021-18-16-252.jpg"
                };

                List<Species> speciesList = new List<Species> {lobo,corco,vibora,garrano,tartanhao,humano};
                
                context.Species.AddRange(
                    speciesList
                );

                Camera lindoso = new Camera("Barragem do lindoso",
                    41.870159, -8.193050, false, false);

                Camera varzea = new Camera("Varzea", 41.902609, -8.220262, true, false);

                Camera bordenca = new Camera("Brande de Bordenca", 41.906355, -8.259469, false, false);

                Camera gorbelas = new Camera("Brande de Gorbelas", 41.941356, -8.270168, false, true);

                Camera campo_soajo = new Camera("Campo Grande de Soajo", 41.882337, -8.227677, true, true);

                Camera paradela = new Camera("Paradela", 41.878339, -8.211230, false, false);

                Camera ermica = new Camera("Ermica", 41.836541, -8.233134, true, false);

                Camera rio_homem = new Camera("Rio Homem", 41.778663, -8.186048, false, false);

                Camera campo_geres = new Camera("Campo do Geres", 41.812498, -8.163385, false, false);

                Camera soajo = new Camera("Soajo", 41.867512, -8.257215, false, false);

                List<Camera> cameraList = new List<Camera>
                    {lindoso, varzea, bordenca, gorbelas, campo_soajo, paradela, ermica, rio_homem, campo_geres, soajo};
                context.Camera.AddRange(
                    cameraList
                );
                context.SaveChanges();
                
                DateTime now = DateTime.Now;

                Sighting sighting1 = new Sighting
                {
                    CaptureMoment = now.AddDays(-1),
                    SpeciesId = lobo.Id,
                    CameraId = bordenca.Id,
                    Quantity = 1
                };
                
                Sighting sighting2 = new Sighting
                {
                    CaptureMoment = now.AddMinutes(-5),
                    SpeciesId = corco.Id,
                    CameraId = gorbelas.Id,
                    Quantity = 6
                };
                
                Sighting sighting3 = new Sighting
                {
                    CaptureMoment = now.AddHours(-1.5),
                    SpeciesId = tartanhao.Id,
                    CameraId = lindoso.Id,
                    Quantity = 1
                };
                
                Sighting sighting4 = new Sighting
                {
                    CaptureMoment = now.AddHours(-3),
                    SpeciesId = humano.Id,
                    CameraId = rio_homem.Id,
                    Quantity = 2
                };
                
                Sighting sighting5 = new Sighting
                {
                    CaptureMoment = now.AddHours(-3),
                    SpeciesId = garrano.Id,
                    CameraId = ermica.Id,
                    Quantity = 3
                };
                
                Sighting sighting6 = new Sighting
                {
                    CaptureMoment = now.AddDays(-2),
                    SpeciesId = vibora.Id,
                    CameraId = campo_soajo.Id,
                    Quantity = 1
                };
                
                Sighting sighting7 = new Sighting
                {
                    CaptureMoment = now.AddDays(-2.2),
                    SpeciesId = lobo.Id,
                    CameraId = campo_geres.Id,
                    Quantity = 1
                };
                
                Sighting sighting8 = new Sighting
                {
                    CaptureMoment = now.AddHours(-0.5),
                    SpeciesId = corco.Id,
                    CameraId = paradela.Id,
                    Quantity = 2
                };
                
                Sighting sighting9 = new Sighting
                {
                    CaptureMoment = now.AddHours(-0.3),
                    SpeciesId = humano.Id,
                    CameraId = lindoso.Id,
                    Quantity = 3
                };

                Sighting sighting10 = new Sighting
                {
                    CaptureMoment = now.AddHours(-4),
                    SpeciesId = garrano.Id,
                    CameraId = campo_geres.Id,
                    Quantity = 2
                };
                
                Sighting sighting11 = new Sighting
                {
                    CaptureMoment = now.AddHours(-2.3),
                    SpeciesId = lobo.Id,
                    CameraId = varzea.Id,
                    Quantity = 2
                };
                
                Sighting sighting12 = new Sighting
                {
                    CaptureMoment = now.AddDays(-5),
                    SpeciesId = vibora.Id,
                    CameraId = varzea.Id,
                    Quantity = 2
                };
                
                Sighting sighting13 = new Sighting
                {
                    CaptureMoment = now.AddDays(-5),
                    SpeciesId = tartanhao.Id,
                    CameraId = soajo.Id,
                    Quantity = 1
                };
                
                context.Sightings.AddRange(
                    sighting1,sighting2,sighting3,sighting4,sighting5,
                    sighting6,sighting7,sighting8, sighting9, sighting10, sighting11,
                    sighting12, sighting13
                );
                
                // creating the root user
                                
                var roleMgr = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userMgr = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                
                var adminRole = new IdentityRole("Admin");
                var rootRole = new IdentityRole("Root");

                //create a role
                roleMgr.CreateAsync(adminRole).GetAwaiter().GetResult();
                roleMgr.CreateAsync(rootRole).GetAwaiter().GetResult();

                //create the root user
                var rootUser = new ApplicationUser
                {
                    UserName = "root",
                    Email = "root@test.com",
                    UseCellphone = false,
                    UseEmail = false,
                    ReceiveSummary = false,
                    PhoneNumber = "-"
                };
                
                var result = userMgr.CreateAsync(rootUser, "password").GetAwaiter().GetResult();
                Console.WriteLine(result.Succeeded);
                
                
                
                //add role to user
                userMgr.AddToRoleAsync(rootUser, adminRole.Name).GetAwaiter().GetResult();
                userMgr.AddToRoleAsync(rootUser, rootRole.Name).GetAwaiter().GetResult();

                context.SaveChanges();
                
                foreach (var fc in cameraList.Select(camera => new FollowedCamera
                {
                    CameraId = camera.Id,
                    UserId = rootUser.Id
                }))
                {
                    context.Add(fc);
                }
                
                foreach (var fs in speciesList.Select(species => new FollowedSpecies()
                {
                    SpeciesId = species.Id,
                    UserId = rootUser.Id
                }))
                {
                    context.Add(fs);
                }
                
                context.SaveChanges();
            }
        }
    }
}