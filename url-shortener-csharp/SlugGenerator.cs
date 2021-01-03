using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace url_shortener_csharp
{
    // this should fulfil 2 contraints: 
    // - as many slugs as possible
    // - in a as few chars as possible
    //
    // 1) what is the max of chars we can allow?
    // this is bound by chars that are allowed in URLs 
    // as per https://www.rfc-editor.org/rfc/rfc1738.txt
    // so eliminating reserved chars we're left with alphanum, case sensitive to increase possibilities,
    // and these -> "$-_.+!*'(),"
    //
    // 2) also relatively simple chars to type on the keyboard or smartphone
    // so possibly drop the "$-_.+!*'(),"
    //
    // 3) how many distinct shortlinks can we have?
    // a-z + A-Z + 0-9 = 62^^X (X char long shortlink)
    // or a-z + A-Z + 0-9 + "$-_.+!*'()," = 72^^X 
    // brushing aside the issue of ambiguous chars, O vs 0 or 1 vs l - not interested in solving this just yet
    //
    // 4) how long shortlinks must be to fit requirements for distinct possibilities.
    // a-z + A-Z + 0-9 = 62^^8 (8 char long shortlink) = 5 trillion - just a round high number + some ballpark estimations
    // or a-z + A-Z + 0-9 + "$-_.+!*'()," = 72^^7 = 5 trillion
    // 
    // that gets me there, but base62 is awkward and .net has URL safe base64 (difference being replacing url unsafe chars)
    // so will implement 8 long with 2 special chars for random generations,
    // but will allow special chars to be used in custom links
    public static class SlugGenerator
    {
        private static long _currentRandomSlugId = 0; // yes, keeping this in memory is daft. manyana, manyana...
        // if server drops, will have to catch up with the random id. how to have this value synched with multiple instances, etc. 
        // for now it's only one, so will see how it goes. 
        private const string Base62Alphanum = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static async Task<string> GenerateRandomSlug(AppDbContext db)
        {
            // dropping re-roll on collision approach in favour of adjusting slug generation as we go along
            // re-rolling would require trip to DB to check for collision, re-roll, recheck. Late in the life of the 
            // product, this would cause a lot of traffic to the db for each call. 
            // for example for 3/4 taken slugs, we'd statistically be hitting collisions 3 out of 4 rolls. 
            // 1 call to generate a slug, 4 trips to db. for 9/10 - 10 trips, etc, leading to all sorts of trouble. 
            //
            // will use base conversion and keep a track on the highest id
            string slug;
            while (true)
            {
                var newId = _currentRandomSlugId++; // this causes first requests to be really slow as it catches up with taken slugs
                // so each new instance would hammer db, until caught up
                // how would i try to do it better? 
                // get it from redis or some other state? 

                slug = Base64UrlEncoder.Encode(newId.ToString());

                // Make sure the slug isn't already used
                Console.WriteLine($"Checking DB for slug collision");
                var isSlugTaken = await db.ShortLinks.AnyAsync(sl => sl.Slug == slug);
                if (!isSlugTaken)
                {
                    break;
                }
                Console.WriteLine($"Slug id {_currentRandomSlugId} with encoded value {slug} was taken, catching up...");
                // ^^ so this is what happens with keeping state in memory. Once caught up it works better than re-roll, but 
                // need to solve for the costly initial catch-up
            }
            Console.WriteLine($"Current random slug ID is {_currentRandomSlugId}");
            return slug;
        }
    }
}