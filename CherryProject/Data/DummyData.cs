using CherryProject.Extension;
using CherryProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Data
{
    public class DummyData
    {
        public async Task<int> run()
        {
            using (var context = new Context())
            {
				// Role setup

				var roles = new []{
					new Role()
					{
						Id                  = Guid.NewGuid().ToString(),
						Name                = "AreaManager",
						NormalizedName      = "AreaManager".ToUpper(),
						ConcurrencyStamp    = Guid.NewGuid().ToString()
					}, new Role()
					{
						Id                  = Guid.NewGuid().ToString(),
						Name                = "Dealer",
						NormalizedName      = "Dealer".ToUpper(),
						ConcurrencyStamp    = Guid.NewGuid().ToString()
					}, new Role()
					{
						Id                  = Guid.NewGuid().ToString(),
						Name                = "DispatchClerk",
						NormalizedName      = "DispatchClerk".ToUpper(),
						ConcurrencyStamp    = Guid.NewGuid().ToString()
					}, new Role()
					{
						Id                  = Guid.NewGuid().ToString(),
						Name                = "SalesOrderOfficer",
						NormalizedName      = "SalesOrderOfficer".ToUpper(),
						ConcurrencyStamp    = Guid.NewGuid().ToString()
					}, new Role()
					{
						Id                  = Guid.NewGuid().ToString(),
						Name                = "SalesManager",
						NormalizedName      = "SalesManager".ToUpper(),
						ConcurrencyStamp    = Guid.NewGuid().ToString()
					}, new Role()
					{
						Id                  = Guid.NewGuid().ToString(),
						Name                = "Storemen",
						NormalizedName      = "Storemen".ToUpper(),
						ConcurrencyStamp    = Guid.NewGuid().ToString()
					}, new Role()
					{
						Id                  = Guid.NewGuid().ToString(),
						Name                = "Administrator",
						NormalizedName      = "Administrator".ToUpper(),
						ConcurrencyStamp    = Guid.NewGuid().ToString()
					}
				};

				var users = new[]{
					new User() 
					{
						Id                      = Guid.NewGuid().ToString(),
						UserName                = "LauraRose20",
						FirstName               = "Jorge",
						LastName                = "Osborne",
						Email                   = "owetivo@iva.za",
						EmailConfirmed          = false,
						PasswordHash            = "GenerDLU1IjbzPTTy5b8U".GetMD5hash(),
						SecurityStamp           = Guid.NewGuid().ToString("D"),
						ConcurrencyStamp        = Guid.NewGuid().ToString(),
						PhoneNumber             = "91358121",
						PhoneNumberConfirmed    = false,
						Region                  = "Kazakhstan",
						Address                 = "",
						Status                  = "Available",
						RoleId                  = roles[0].Id
					},new User()
					{
						Id                      = Guid.NewGuid().ToString(),
						UserName                = "MarvinRomero76",
						FirstName               = "Essie",
						LastName                = "Harper",
						Email                   = "dadkab@jutmu.cf",
						EmailConfirmed          = false,
						PasswordHash            = "DouglaseH54UzHa650rf0G".GetMD5hash(),
						SecurityStamp           = Guid.NewGuid().ToString("D"),
						ConcurrencyStamp        = Guid.NewGuid().ToString(),
						PhoneNumber             = "37574962",
						PhoneNumberConfirmed    = false,
						Region                  = "Germany",
						Address                 = "",
						Status                  = "Available",
						RoleId                  = roles[1].Id
					},new User()
					{
						Id                      = Guid.NewGuid().ToString(),
						UserName                = "CoreySimon99",
						FirstName               = "Elva",
						LastName                = "Simmons",
						Email                   = "femolsum@ictenib.us",
						EmailConfirmed          = false,
						PasswordHash            = "CatherineUucWjWm".GetMD5hash(),
						SecurityStamp           = Guid.NewGuid().ToString("D"),
						ConcurrencyStamp        = Guid.NewGuid().ToString(),
						PhoneNumber             = "62166232",
						PhoneNumberConfirmed    = false,
						Region                  = "Ethiopia",
						Address                 = "",
						Status                  = "Available",
						RoleId                  = roles[1].Id
					},new User()
					{
						Id                      = Guid.NewGuid().ToString(),
						UserName                = "LarryFuller54",
						FirstName               = "Myrtie",
						LastName                = "Willis",
						Email                   = "karjod@igo.sa",
						EmailConfirmed          = false,
						PasswordHash            = "MatthewZp8XwE7a8xZwmaY".GetMD5hash(),
						SecurityStamp           = Guid.NewGuid().ToString("D"),
						ConcurrencyStamp        = Guid.NewGuid().ToString(),
						PhoneNumber             = "54739838",
						PhoneNumberConfirmed    = false,
						Region                  = "Estonia",
						Address                 = "",
						Status                  = "Available",
						RoleId                  = roles[1].Id
					},new User()
					{
						Id                      = Guid.NewGuid().ToString(),
						UserName                = "HuldaChandler57",
						FirstName               = "Jesse",
						LastName                = "Holt",
						Email                   = "kiizupe@gegew.cd",
						EmailConfirmed          = false,
						PasswordHash            = "BrettjftHZ9qVJcDUSc41ziT3".GetMD5hash(),
						SecurityStamp           = Guid.NewGuid().ToString("D"),
						ConcurrencyStamp        = Guid.NewGuid().ToString(),
						PhoneNumber             = "32909942",
						PhoneNumberConfirmed    = false,
						Region                  = "Burkina Faso",
						Address                 = "",
						Status                  = "Available",
						RoleId                  = roles[1].Id
					},new User()
					{
						Id                      = Guid.NewGuid().ToString(),
						UserName                = "LeonaKlein87",
						FirstName               = "Amelia",
						LastName                = "Norman",
						Email                   = "luhbituw@gehtawdo.ax",
						EmailConfirmed          = false,
						PasswordHash            = "CalliekYkML".GetMD5hash(),
						SecurityStamp           = Guid.NewGuid().ToString("D"),
						ConcurrencyStamp        = Guid.NewGuid().ToString(),
						PhoneNumber             = "54046186",
						PhoneNumberConfirmed    = false,
						Region                  = "Antigua & Barbuda",
						Address                 = "",
						Status                  = "Available",
						RoleId                  = roles[1].Id
					},new User()
					{
						Id                      = Guid.NewGuid().ToString(),
						UserName                = "AlejandroSimmons19",
						FirstName               = "Mildred",
						LastName                = "Young",
						Email                   = "afu@dituwkim.gb",
						EmailConfirmed          = false,
						PasswordHash            = "BernardpWMzYYVYJMo".GetMD5hash(),
						SecurityStamp           = Guid.NewGuid().ToString("D"),
						ConcurrencyStamp        = Guid.NewGuid().ToString(),
						PhoneNumber             = "38203788",
						PhoneNumberConfirmed    = false,
						Region                  = "Aruba",
						Address                 = "",
						Status                  = "Available",
						RoleId                  = roles[1].Id
					},new User()
					{
						Id                      = Guid.NewGuid().ToString(),
						UserName                = "DouglasHughes62",
						FirstName               = "Terry",
						LastName                = "King",
						Email                   = "pecauzi@ata.gs",
						EmailConfirmed          = false,
						PasswordHash            = "MiltonBPYeUZzpuAbSe".GetMD5hash(),
						SecurityStamp           = Guid.NewGuid().ToString("D"),
						ConcurrencyStamp        = Guid.NewGuid().ToString(),
						PhoneNumber             = "62518155",
						PhoneNumberConfirmed    = false,
						Region                  = "Palestinian Territories",
						Address                 = "",
						Status                  = "Available",
						RoleId                  = roles[2].Id
					},new User()
					{
						Id                      = Guid.NewGuid().ToString(),
						UserName                = "LloydWheeler59",
						FirstName               = "Theresa",
						LastName                = "Carr",
						Email                   = "fogo@wudoh.gg",
						EmailConfirmed          = false,
						PasswordHash            = "DanielF6fxaW8aQ7fwrI".GetMD5hash(),
						SecurityStamp           = Guid.NewGuid().ToString("D"),
						ConcurrencyStamp        = Guid.NewGuid().ToString(),
						PhoneNumber             = "96168563",
						PhoneNumberConfirmed    = false,
						Region                  = "British Virgin Islands",
						Address                 = "",
						Status                  = "Available",
						RoleId                  = roles[3].Id
					},new User()
					{
						Id                      = Guid.NewGuid().ToString(),
						UserName                = "MichaelChambers36",
						FirstName               = "Jeremiah",
						LastName                = "Murphy",
						Email                   = "tijbet@az.by",
						EmailConfirmed          = false,
						PasswordHash            = "LucasOHfuKhaIbQ6c5mUbd".GetMD5hash(),
						SecurityStamp           = Guid.NewGuid().ToString("D"),
						ConcurrencyStamp        = Guid.NewGuid().ToString(),
						PhoneNumber             = "79604847",
						PhoneNumberConfirmed    = false,
						Region                  = "Vietnam",
						Address                 = "",
						Status                  = "Available",
						RoleId                  = roles[4].Id
					},new User()
					{
						Id                      = Guid.NewGuid().ToString(),
						UserName                = "GeorgiaHarper96",
						FirstName               = "Raymond",
						LastName                = "Parsons",
						Email                   = "wiuc@dud.ca",
						EmailConfirmed          = false,
						PasswordHash            = "HenriettaPOFqLZk4G".GetMD5hash(),
						SecurityStamp           = Guid.NewGuid().ToString("D"),
						ConcurrencyStamp        = Guid.NewGuid().ToString(),
						PhoneNumber             = "47008943",
						PhoneNumberConfirmed    = false,
						Region                  = "American Samoa",
						Address                 = "",
						Status                  = "Available",
						RoleId                  = roles[5].Id
					},new User()
					{
						Id                      = Guid.NewGuid().ToString(),
						UserName                = "admin",
						FirstName               = "Ho Ching",
						LastName                = "Yeung",
						Email                   = "c010ur1355code@gmail.com",
						EmailConfirmed          = false,
						PasswordHash            = "admin".GetMD5hash(),
						SecurityStamp           = Guid.NewGuid().ToString("D"),
						ConcurrencyStamp        = Guid.NewGuid().ToString(),
						PhoneNumber             = "47008943",
						PhoneNumberConfirmed    = false,
						Region                  = "Hong Kong",
						Address                 = "None Of Your Business.",
						Status                  = "Available",
						RoleId                  = roles[6].Id
					}
				};

				await context.Role.AddRangeAsync(roles);
				await context.User.AddRangeAsync(users);

				return await context.SaveChangesAsync();
            }
        }
    }
}
