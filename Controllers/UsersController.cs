using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using API;
using Google.Cloud.Firestore;

namespace API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private FirestoreDb db;

        public UsersController()
        {
            string projectId = "bsynchro-734c5";
            string jsonPath = "C:/Users/Daher/source/repos/API/API/bsynchro-734c5-firebase-adminsdk-623wa-5e8587e9b0.json";

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", jsonPath);
            db = FirestoreDb.Create(projectId);
        }
        private static List<User> users = new List<User>();

        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            return users;
        }

        [HttpGet("{customerId}")]
        public async Task<ActionResult<User>> GetByCustomerId(int customerId)
        {
            CollectionReference usersCollection = db.Collection("UsersCollection");

            Query query = usersCollection.WhereEqualTo("CustomerId", customerId);

            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Count == 0)
            {
                return NotFound(); 
            }

            DocumentSnapshot documentSnapshot = snapshot.First();
            var user = documentSnapshot.ConvertTo<User>();

            return user;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            CollectionReference usersCollection = db.Collection("UsersCollection");
            Query existingUserQuery = usersCollection.WhereEqualTo("CustomerId", user.CustomerId);

            QuerySnapshot existingUserSnapshot = await existingUserQuery.GetSnapshotAsync();

            if (existingUserSnapshot.Count > 0)
            {
                return Conflict("User with the same CustomerId already exists.");
            }

            DocumentReference userDocument = usersCollection.Document();

            await userDocument.SetAsync(user);


            return CreatedAtAction("GetByCustomerId", new { customerId = user.CustomerId }, user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserByCustomerId(int customerId, [FromBody] User updatedUser)
        {
            CollectionReference usersCollection = db.Collection("UsersCollection");

            Query query = usersCollection.WhereEqualTo("CustomerId", customerId);

            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Count == 0)
            {
                return NotFound();
            }

            DocumentSnapshot documentSnapshot = snapshot.First();
            var userDocument = documentSnapshot.Reference;

            // Update the user with the provided data
            await userDocument.SetAsync(updatedUser, SetOptions.MergeAll);

            return NoContent();
        }

        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteUserByCustomerId(int customerId)
        {
            CollectionReference usersCollection = db.Collection("UsersCollection");

            Query query = usersCollection.WhereEqualTo("CustomerId", customerId);

            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Count == 0)
            {
                return NotFound();
            }

            DocumentSnapshot documentSnapshot = snapshot.First();
            var userDocument = documentSnapshot.Reference;

            await userDocument.DeleteAsync();

            return NoContent();
        }

    }
}
