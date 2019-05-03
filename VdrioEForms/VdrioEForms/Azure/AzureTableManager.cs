using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.EFForms;
using VdrioEForms.UserManagement;

namespace VdrioEForms.Azure
{
    public class AzureTableManager
    {
        public static bool IsAppTester = false;
        public static CloudStorageAccount storageAccount;
        public static CloudTableClient tableClient;
        public static CloudTable formTable, userTable;
        public static List<CloudTable> formSubTables;
        public static List<EFUser> CurrentUsers;
        static bool Initialized;

        public static void InitializeClientService()
        {
            storageAccount = new CloudStorageAccount(new StorageCredentials("bwltestingstorage", "c/FpR400/p36lqlnjtpuykzlvmNKNCrW5pVkVa7giiZppRUQdGa3+UNmKdO0lH/5Bot1r2hDicqEmpV2oaR4eg=="), true);
            tableClient = storageAccount.CreateCloudTableClient();
            formTable = tableClient.GetTableReference("BlackrockForms");
            userTable = tableClient.GetTableReference("BlackrockUsers");
            formSubTables = new List<CloudTable>();
            CreateIfNotExistAllTables();
            Initialized = true;
            IsAppTester = false;
            //GetSubTables();
        }

        public static void InitializeAppTesterService()
        {
            storageAccount = new CloudStorageAccount(new StorageCredentials("bwltestingstorage", "c/FpR400/p36lqlnjtpuykzlvmNKNCrW5pVkVa7giiZppRUQdGa3+UNmKdO0lH/5Bot1r2hDicqEmpV2oaR4eg=="), true);
            tableClient = storageAccount.CreateCloudTableClient();
            formTable = tableClient.GetTableReference("EFTestForms");
            userTable = tableClient.GetTableReference("EFTestUsers");
            CreateIfNotExistAllTables();
            IsAppTester = true;
            //GetSubTables();
        }

        public static async void GetSubTables()
        {
            List<EFForm> forms = await GetAllForms();
            foreach(EFForm f in forms)
            {
                CloudTable c;
                if (IsAppTester)
                {
                    c = tableClient.GetTableReference("Test" + f.FormName.Replace(" ", "") + "Table");
                }
                else
                {
                    c = tableClient.GetTableReference(f.FormName.Replace(" ", "") + "Table");
                }
                
                await c.CreateIfNotExistsAsync();
                formSubTables.Add(c);
            }
        }

        public static async void CreateIfNotExistAllTables()
        {
            await formTable.CreateIfNotExistsAsync();
            await userTable.CreateIfNotExistsAsync();
        }

        public static async Task<EFUser> Login(string userEntry, string password, bool retrying = false)
        {
            if (!Initialized)
            {
                InitializeClientService();
            }
            CurrentUsers = await GetAllUsers();
            foreach(EFUser u in CurrentUsers)
            {
                u.DecryptUser();
            }
            EFUser user = CurrentUsers.Find(x => x.Email == userEntry);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.Password))
                {
                    if (user.Password == password && user.Active && !user.Deleted)
                    {
                        
                        user.DecryptUser();
                        return await GetUser("", userEntry);
                    }
                }
                else
                {
                    
                    return user;
                }
            }
            else
            {
                user = CurrentUsers.Find(x => x.UserName == userEntry);
                
                if (user != null)
                {               
                    if (user.Password == password && user.Active && !user.Deleted)
                    {
                     
                        user.DecryptUser();
                        return await GetUser(userEntry);
                    }
                }
            }
            if (!retrying)
            {
                if (IsAppTester)
                {
                    InitializeClientService();
                }
                else
                {
                    InitializeAppTesterService();
                }
                return await Login(userEntry, password, true);
            }
            else
            {
                if (IsAppTester)
                {
                    InitializeClientService();
                }
                else
                {
                    InitializeAppTesterService();
                }
                return null;
            }
            
        }

        public static void RequestSetup()
        {

        }

        public static async Task<EFUser> Setup(string email,string userName, string firstName, string lastName, string password, string confirmPassword)
        {
            if (!Initialized)
            {
                InitializeClientService();
            }
            EFUser u = await GetUser("", email);
            u.UserName = userName;
            u.FirstName = firstName;
            u.LastName = lastName;
            u.Password = password;
            return await UpdateUser(u);

        }

        public static async Task<List<EFUser>> GetAllUsers(bool includeInactive = false)
        {
            Debug.WriteLine("Getting all users for ya");
            if (!Initialized )
            {
                InitializeClientService();
            }
            try
            {
                TableQuery<EFUser> query = new TableQuery<EFUser>();
                if (!includeInactive)
                    query = new TableQuery<EFUser>().Where(TableQuery.GenerateFilterConditionForBool("Deleted", QueryComparisons.Equal, includeInactive));

                TableContinuationToken token = null;
                List<EFUser> users = new List<EFUser>();
                do
                {
                    TableQuerySegment<EFUser> tableEntities = await userTable.ExecuteQuerySegmentedAsync(query, token);
                    token = tableEntities.ContinuationToken;
                    foreach (EFUser u in tableEntities.Results)
                    {
                        u.DecryptUser();
                        users.Add(u);
                    }
                    if (LoginPage.CurrentUser?.UserType == 2)
                    {
                        if (IsAppTester)
                        {
                            InitializeClientService();
                        }
                        else
                        {
                            InitializeAppTesterService();
                        }
                        tableEntities = await userTable.ExecuteQuerySegmentedAsync(query, token);
                        token = tableEntities.ContinuationToken;
                        foreach (EFUser u in tableEntities.Results)
                        {
                            u.DecryptUser();
                            users.Add(u);
                        }
                    }
                    
                } while (token != null);
                Debug.WriteLine("Users found: " + users.Count);
                
                return users;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get all users: " + ex);
                return null;
            }
        }

        public static async Task<EFUser> UpdateUser(EFUser user)
        {
            if (!Initialized)
            {
                InitializeClientService();
            }
            try
            {
                await userTable.CreateIfNotExistsAsync();
                user.EncryptUser();
                //user.PartitionKey = user.FirstName + user.LastName;
                if (user.UserType == 3 || user.UserType == 4)
                {
                    InitializeAppTesterService();
                }
                else
                {
                    InitializeClientService();
                }
                List<EFUser> users = await GetAllUsers();
                if (users.Find(x => x.RowKey!=user.RowKey&&(x.UserName == user.UserName || x.Email == user.Email)) != null)
                {
                    Debug.WriteLine("User already exists");
                    return null;
                }

                //user.Token = CreateSecureToken(6);
                TableOperation operation = TableOperation.InsertOrReplace(user);
                TableResult result = await userTable.ExecuteAsync(operation);
                return await GetUser("", user.Email);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to add user: " + ex);
                return null;
            }
        }

        public static async Task<EFUser> GetUser(string userName = "", string email = "")
        {
            
            Debug.WriteLine("Getting a user");
            string id;
            if (string.IsNullOrEmpty(userName))
            {
                if (string.IsNullOrEmpty(email))
                {
                    return null;
                }
                else
                {
                    id = email;
                }
            }
            else
            {
                id = userName;
            }
            if (!Initialized)
            {
                InitializeClientService();
            }
            try
            {
                TableQuery<EFUser> query;
                query = new TableQuery<EFUser>();


                TableContinuationToken token = null;
                List<EFUser> users = await GetAllUsers();
                return users.Find(x=>x.UserName == id || x.Email == id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get user: " + ex);
                return null;
            }
        }

        public static async Task<List<EFForm>> GetAllForms(bool includeInactive = false)
        {
            Debug.WriteLine("Getting all forms for ya");
            if (!Initialized)
            {
                InitializeClientService();
            }
            if (LoginPage.CurrentUser.UserType == 2)
            {
                InitializeClientService();
            }
            try
            {
                TableQuery<EFForm> query;
                query = new TableQuery<EFForm>();
                

                TableContinuationToken token = null;
                List<EFForm> forms = new List<EFForm>();
                do
                {
                    TableQuerySegment<EFForm> tableEntities = await formTable.ExecuteQuerySegmentedAsync(query, token);
                    token = tableEntities.ContinuationToken;
                    foreach (EFForm f in tableEntities.Results)
                    {
                        f.DecryptForm();
                        foreach (EFEntry e in f.Entries)
                        {
                            e.DecryptEntry();
                        }
                        f.OriginalUser?.DecryptUser();
                        f.LastModifiedUser?.DecryptUser();
                        forms.Add(f);

                    }

                } while (token != null);
                Debug.WriteLine("Forms found: " + forms.Count);
                foreach (EFForm f in forms)
                {
                    Debug.WriteLine("Found a form: " + f.FormName);
                }
                return forms;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get all users: " + ex);
                return null;
            }
        }

        public static async Task<List<EFForm>> GetAllFormSubmissions(string tableName, DateTime from, DateTime to)
        {
            Debug.WriteLine("Getting those form submssions");
            if (!Initialized)
            {
                InitializeClientService();
            }
            if(LoginPage.CurrentUser.UserType == 2)
            {
                InitializeClientService();
            }
            try
            {
                TableQuery<EFForm> query;
                Debug.WriteLine(from.Ticks.ToString());
                Debug.WriteLine(to.Ticks.ToString());
                /*string final = TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("TimeInTicks", QueryComparisons.GreaterThanOrEqual, from.Ticks.ToString()), TableOperators.And,
                   TableQuery.GenerateFilterCondition("TimeInTicks", QueryComparisons.LessThanOrEqual, to.Ticks.ToString()));*/
                string fromString = TableQuery.GenerateFilterConditionForLong("TimeInTicks", QueryComparisons.GreaterThanOrEqual, from.Ticks);
                string toString = TableQuery.GenerateFilterConditionForLong("TimeInTicks", QueryComparisons.LessThanOrEqual, to.Ticks);
                string final = TableQuery.CombineFilters(fromString, TableOperators.And, toString);
                query = new TableQuery<EFForm>().Where(final);
                /*query = new TableQuery<EFForm>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("TimeInTicks", QueryComparisons.GreaterThanOrEqual, from.Ticks.ToString()), TableOperators.And,
                   TableQuery.GenerateFilterCondition("TimeInTicks", QueryComparisons.LessThanOrEqual, to.Ticks.ToString())));*/


                TableContinuationToken token = null;
                List<EFForm> forms = new List<EFForm>();
                do
                {
                    CloudTable c = tableClient.GetTableReference(tableName);
                    await c.CreateIfNotExistsAsync();
                    TableQuerySegment<EFForm> tableEntities = await c.ExecuteQuerySegmentedAsync(query, token);
                    token = tableEntities.ContinuationToken;
                    foreach (EFForm f in tableEntities.Results)
                    {
                        f.DecryptForm();
                        foreach(EFEntry e in f.Entries)
                        {
                            e.DecryptEntry();
                        }
                        f.OriginalUser.DecryptUser();
                        f.LastModifiedUser.DecryptUser();
                        forms.Add(f);

                    }

                } while (token != null);

                forms = forms.OrderByDescending(f=>f.TimeInTicks).ToList();
                return forms;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get all form submissions: " + ex);
                return null;
            }
        }

        public static async Task<EFUser> AddUser(EFUser user)
        {
            if (!Initialized)
            {
                InitializeClientService();
            }
            try
            {
                //await userTable.CreateIfNotExistsAsync();
                user.EncryptUser();
                user.RowKey = Guid.NewGuid().ToString();
                user.PartitionKey = user.FirstName+user.LastName;

                    List<EFUser> users = await GetAllUsers();
                if (users.Find(x=>x.UserName == user.UserName || x.Email == user.Email) != null)
                {
                    Debug.WriteLine("User already exists");
                    return null;
                }
                if (user.UserType == 3 || user.UserType == 4)
                {
                    InitializeAppTesterService();
                }
                else
                {
                    InitializeClientService();
                }
                //user.Token = CreateSecureToken(6);
                TableOperation operation = TableOperation.Insert(user);
                TableResult result = await userTable.ExecuteAsync(operation);
                return await GetUser(user.Email);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to add user: " + ex);
                return null;
            }
        }

        public static async Task<EFForm> AddForm(EFForm form)
        {
            if (!Initialized)
            {
                InitializeClientService();
            }
            try
            {
                await formTable.CreateIfNotExistsAsync();
                form.RowKey = Guid.NewGuid().ToString();             
                form.TableName = form.FormName.Replace(" ", "") + "Table";
                form.PartitionKey = form.TableName;
                //Debug.WriteLine("Table Name: " + form.TableName);
                if (IsAppTester)
                {
                    form.TableName = "Test" + form.TableName;
                }
                form.EncryptForm();
                List<EFForm> forms = await GetAllForms();

                if (forms.Find(x => x.FormName == form.FormName) != null)
                {
                    Debug.WriteLine("Form already exists");
                    return null;
                }
                CloudTable c = tableClient.GetTableReference(form.TableName);
                await c.CreateIfNotExistsAsync();


                //user.Token = CreateSecureToken(6);
                TableOperation operation = TableOperation.Insert(form);
                TableResult result = await formTable.ExecuteAsync(operation);

                return form;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to add form: " + ex);
                return null;
            }
        }

        public static async Task<EFForm> UpdateForm(EFForm form)
        {
            if (!Initialized)
            {
                InitializeClientService();
            }
            try
            {
                await userTable.CreateIfNotExistsAsync();
                form.EncryptForm();

                CloudTable c = tableClient.GetTableReference(form.TableName);
                await c.CreateIfNotExistsAsync();

                //user.Token = CreateSecureToken(6);
                TableOperation operation = TableOperation.InsertOrReplace(form);
                TableResult result = await formTable.ExecuteAsync(operation);

                return form;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to add form: " + ex);
                return null;
            }
        }

        public static async Task<EFForm> UpdateFormSubmission(EFForm form)
        {
            if (!Initialized)
            {
                InitializeClientService();
            }
            try
            {
                await userTable.CreateIfNotExistsAsync();
                form.EncryptForm();

                CloudTable c = tableClient.GetTableReference(form.TableName);
                await c.CreateIfNotExistsAsync();

                //user.Token = CreateSecureToken(6);
                TableOperation operation = TableOperation.InsertOrReplace(form);
                TableResult result = await c.ExecuteAsync(operation);

                return form;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to add form: " + ex);
                return null;
            }
        }

        public static async Task<EFForm> AddFormSubmission(EFForm form)
        {
            if (!Initialized)
            {
                InitializeClientService();
            }
            try
            {
                await userTable.CreateIfNotExistsAsync();
                form.EncryptForm();
                form.RowKey = Guid.NewGuid().ToString();
                form.PartitionKey = LoginPage.CurrentUser.FirstName.Replace(" ", "") + LoginPage.CurrentUser.LastName.Replace(" ", "") + "Submissions";
                form.TimeInTicks = DateTime.Now.Ticks;
                Debug.WriteLine(form.TableName);
                CloudTable c = tableClient.GetTableReference(form.TableName);
                await c.CreateIfNotExistsAsync();
                /*if (c == null)
                {
                    Debug.WriteLine("Unable to find table of name");
                    return null;
                }*/

                //user.Token = CreateSecureToken(6);
                TableOperation operation = TableOperation.Insert(form);
                TableResult result = await c.ExecuteAsync(operation);

                return form;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to add user: " + ex);
                return null;
            }
        }

        readonly static CloudStorageAccount _cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=bwltestingstorage;AccountKey=c/FpR400/p36lqlnjtpuykzlvmNKNCrW5pVkVa7giiZppRUQdGa3+UNmKdO0lH/5Bot1r2hDicqEmpV2oaR4eg==;EndpointSuffix=core.windows.net");
        readonly static CloudBlobClient _blobClient = _cloudStorageAccount.CreateCloudBlobClient();
        public static async Task<List<T>> GetBlobs<T>(string containerName, string prefix = "", int? maxresultsPerQuery = null, BlobListingDetails blobListingDetails = BlobListingDetails.None) where T : ICloudBlob
        {
            var blobContainer = _blobClient.GetContainerReference(containerName);

            var blobList = new List<T>();
            BlobContinuationToken continuationToken = null;

            try
            {
                do
                {
                    var response = await blobContainer.ListBlobsSegmentedAsync(prefix, true, blobListingDetails, maxresultsPerQuery, continuationToken, null, null);

                    continuationToken = response?.ContinuationToken;

                    foreach (var blob in response?.Results?.OfType<T>())
                    {
                        blobList.Add(blob);

                    }

                } while (continuationToken != null);
            }
            catch (Exception ex)
            {

                //Handle Exception
            }

            return blobList;
        }

        public static async Task<CloudBlockBlob> SaveBlockBlob(string containerName, byte[] blob, string blobTitle)
        {
            var blobContainer = _blobClient.GetContainerReference(containerName);
            if ((await GetBlobs<CloudBlockBlob>("documents")).Where(x => x.Name == blobTitle).ToList().Count > 0)
            {
                Debug.WriteLine("this file already exists");
                return null;
            }
            else
            {
                var blockBlob = blobContainer.GetBlockBlobReference(blobTitle);
                await blockBlob.UploadFromByteArrayAsync(blob, 0, blob.Length);

                return blockBlob;
            }

        }


    }
}
