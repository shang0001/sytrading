using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SYTrading.Models;

namespace SYTrading.DAL
{
    public class BusinessDBInitializer : DropCreateDatabaseIfModelChanges<BusinessDbContext>
    {
        protected override void Seed(BusinessDbContext context)
        {
            var material = new List<Material>
            {
                new Material { Name = "Nitrile",   Description = "dsadsada" },
                new Material { Name = "Latex", Description = "dsadsada" },
                new Material { Name = "PVC",   Description = "dsadsada" },
                new Material { Name = "Neoprene",    Description = "dsadsada" },
                new Material { Name = "Butyl",      Description = "dsadsada" },
                new Material { Name = "Viton",    Description = "dsadsada" },
                new Material { Name = "PU",    Description = "dsadsada" }
            };
            material.ForEach(s => context.Materials.Add(s));
            context.SaveChanges();

            var glove = new List<Glove>
            {
                new Glove { ItemNo = "F230", Name = "Glove 1", Description = "dasdsa", MaterialID = 1, Sizes = "S, M, L", Colors = "White, Red, Black" },
                new Glove { ItemNo = "F200K", Name = "Glove 2", Description = "dasdsa", MaterialID = 2, Sizes = "S, M, L", Colors = "White, Red, Black" },
                new Glove { ItemNo = "JK213", Name = "Glove 3", Description = "dasdsa", MaterialID = 3, Sizes = "S, M, L", Colors = "White, Red, Black" },
                new Glove { ItemNo = "QI98", Name = "Glove 4", Description = "dasdsa", MaterialID = 4, Sizes = "S, M, L", Colors = "White, Red, Black" },
                new Glove { ItemNo = "U23", Name = "Glove 5", Description = "dasdsa", MaterialID = 5, Sizes = "S, M, L", Colors = "White, Red, Black" },
                new Glove { ItemNo = "RR45", Name = "Glove 6", Description = "dasdsa", MaterialID = 6, Sizes = "S, M, L", Colors = "White, Red, Black" }
            };
            glove.ForEach(s => context.Gloves.Add(s));
            context.SaveChanges();

            var application = new List<Application>
            {
                new Application { Name = "Red meat and poultry processing", Category = "dasdsa" },
                new Application { Name = "Chemical processing", Category = "dasdsa" },
                new Application { Name = "Machining operations using cutting oil", Category = "dasdsa" },
                new Application { Name = "Battery manufacturing", Category = "dasdsa" },
                new Application { Name = "Chemical Industry", Category = "dasdsa" },
                new Application { Name = "Commercial fishing", Category = "dasdsa" },
                new Application { Name = "Suitable for handling oils, acids, caustics and organic solvents", Category = "dasdsa" }
            };
            application.ForEach(s => context.Applications.Add(s));
            context.SaveChanges();

            var gloveApplications = new List<GloveApplication>
            {
                new GloveApplication { GloveID = 1, ApplicationID = 1 },
                new GloveApplication { GloveID = 2, ApplicationID = 1 },
                new GloveApplication { GloveID = 2, ApplicationID = 2 },
                new GloveApplication { GloveID = 2, ApplicationID = 3 },
                new GloveApplication { GloveID = 3, ApplicationID = 3 },
                new GloveApplication { GloveID = 3, ApplicationID = 4 },
                new GloveApplication { GloveID = 4, ApplicationID = 4 },
                new GloveApplication { GloveID = 5, ApplicationID = 5 },
                new GloveApplication { GloveID = 5, ApplicationID = 6 },
                new GloveApplication { GloveID = 6, ApplicationID = 1 },
                new GloveApplication { GloveID = 6, ApplicationID = 7 }
            };
            gloveApplications.ForEach(s => context.GloveApplications.Add(s));
            context.SaveChanges();
        }
    }
}