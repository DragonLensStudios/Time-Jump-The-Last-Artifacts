using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PXE.Core.Data_Persistence.Interfaces;
using PXE.Core.SerializableTypes;
using PXE.Core.Utilities.Json;
using Unity.Properties;
using UnityEngine;

namespace PXE.Core.Data_Persistence
{
    //TODO: Add handling for recovery and merging same files with same name but different data and corrupt timestamp
    /// <summary>
    ///  Handles the loading and saving of game data to and from files.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FileDataHandler : IFileDataHandler
    {
        /// <summary>
        /// Full path to the data directory where profiles are saved. 
        /// </summary>
        [field: Tooltip("The path to the folder where the data will be saved.")]
        [field: SerializeField] public virtual string DataPath { get; set; }
        
        /// <summary>
        /// Name of the data file for each profile.
        /// </summary>
        [field: Tooltip("The name of the data file.")]
        [field: SerializeField] public virtual string FileName { get; set; } = "Save";
        
        
        /// <summary>
        ///  Extension for the data file.
        /// </summary>
        [field: Tooltip("The extension for the data file.")]
        [field: SerializeField] public virtual string Extension { get; set; } = ".save";
        
        /// <summary>
        /// Whether to encrypt/decrypt data when reading/writing.
        /// </summary>
        [field: Tooltip("Whether or not to encrypt the data.")]
        [field: SerializeField] public virtual bool UseEncryption { get; set; } = false;

        /// <summary>
        /// Encryption key used if encryption is enabled.
        /// Should be kept private.
        /// </summary>
        [field: Tooltip("The encryption key used if encryption is enabled.")]
        [field: SerializeField] public virtual string EncryptionCodeWord { get; set; } = "53cur3YK37W0rd";

        /// <summary>
        /// File extension used for backup files.
        /// </summary>
        [field: Tooltip("The extension for the backup file.")]
        [field: SerializeField] public virtual string BackupExtension { get; set; } = ".bak";


        /// <summary>
        ///  Constructor for FileDataHandler class.
        /// </summary>
        public FileDataHandler()
        {
            
        }
        
        /// <summary>
        ///  Constructor for FileDataHandler class.
        /// </summary>
        /// <param name="dataPath"></param>
        /// <param name="fileName"></param>
        /// <param name="extension"></param>
        /// <param name="useEncryption"></param>
        public FileDataHandler(string dataPath, string fileName, string extension, bool useEncryption) 
        {
            DataPath = dataPath;
            FileName = fileName;
            Extension = extension;
            UseEncryption = useEncryption;
        }

        public void Initialize()
        {
            if (string.IsNullOrWhiteSpace(DataPath))
            {
                DataPath = Application.persistentDataPath;
            }
        }
        
        public void Initialize(string path, string fileName, string extension, bool useEncryption, string encryptionCodeWord, string backupExtension)
        {
            DataPath = path;
            FileName = fileName;
            Extension = extension;
            UseEncryption = useEncryption;
            EncryptionCodeWord = encryptionCodeWord;
            BackupExtension = backupExtension;
        }

        //TODO: Check to make sure that large files can be saved and if it locks the game so that the save process can happen and prevent data from changing during the save process
        public virtual void Save<T>(List<T> data, SerializableGuid playerID, string playerName) where T : class, IGameDataContent, new()
        {
            // base case - if the profileId is null, return right away
            if (SerializableGuid.IsEmpty(playerID)) 
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                return;
            }

            if (data.Count <= 0)
            {
                return;
            }

            // use Path.Combine to account for different OS's having different path separators
            string fullPath = Path.Combine(DataPath, playerName + ";" + playerID.Guid, FileName + Extension);
            string backupFilePath = fullPath + BackupExtension;
            
            try 
            {
                if(string.IsNullOrWhiteSpace(fullPath)) return;
                // create the directory the file will be written to if it doesn't already exist
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? throw new InvalidPathException($"Invalid path: {fullPath}"));

                // serialize the list of C# game data objects into Json
                string dataToStore = JsonConvert.SerializeObject(data, Formatting.Indented, JsonUtilities.JsonSettingsForUnityObjects());

                // optionally encrypt the data
                if (UseEncryption) 
                {
                    dataToStore = EncryptDecrypt(dataToStore);
                }

                // write the serialized data to the file
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream)) 
                    {
                        writer.Write(dataToStore);
                    }
                }

                // verify the newly saved file can be loaded successfully
                List<T> verifiedGameDataList = Load<T>(playerID, playerName);
                // if the data can be verified, back it up
                if (verifiedGameDataList != null && verifiedGameDataList.Count > 0) 
                {
                    File.Copy(fullPath, backupFilePath, true);
                }
                // otherwise, something went wrong and we should throw an exception
                else 
                {
                    throw new Exception("Save file could not be verified and backup could not be created.");
                }

            }
            catch (Exception e) 
            {
                Debug.LogError($"Error occurred when trying to save data to file: {fullPath}\n{e.Message}");
            }
        }

        public virtual List<T> Load<T>(SerializableGuid playerID, string playerName, bool allowRestoreFromBackup = true) where T : class, IGameDataContent, new()
        {
            // base case - if the profileId is null, return right away
            if (SerializableGuid.IsEmpty(playerID)) 
            {
                return default;
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                return default;
            }

            // use Path.Combine to account for different OS's having different path separators
            string fullPath = Path.Combine(DataPath, playerName + ";" + playerID.Guid, FileName + Extension);
            List<T> loadedData = default(List<T>);
            if (File.Exists(fullPath))
            {
                try 
                {
                    // load the serialized data from the file
                    string dataToLoad = "";
                    using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    // optionally decrypt the data
                    if (UseEncryption) 
                    {
                        dataToLoad = EncryptDecrypt(dataToLoad);
                    }

                    // deserialize the data from Json back into a list of C# objects
                    loadedData = JsonConvert.DeserializeObject<List<T>>(dataToLoad, JsonUtilities.JsonSettingsForUnityObjects());
                }
                catch (Exception e) 
                {
                    // since we're calling Load(..) recursively, we need to account for the case where
                    // the rollback succeeds, but data is still failing to load for some other reason,
                    // which without this check may cause an infinite recursion loop.
                    if (allowRestoreFromBackup) 
                    {
                        Debug.LogWarning($"Failed to load data file. Attempting to roll back.\n{e.Message}");
                        bool rollbackSuccess = AttemptRollback(fullPath);
                        if (rollbackSuccess)
                        {
                            // try to load again recursively
                            loadedData = Load<T>(playerID, playerName, false);
                        }
                    }
                    // if we hit this else block, one possibility is that the backup file is also corrupt
                    else 
                    {
                        Debug.LogError($"Error occurred when trying to load file at path: {fullPath} and backup did not work.\n {e.Message}");
                    }
                }
            }
            return loadedData;        
        }


        // public virtual void Save<T>(T data, SerializableGuid playerID, string playerName) where T : class, IGameData<T>
        // {
        //     // base case - if the profileId is null, return right away
        //     if (playerID.Guid == Guid.Empty) 
        //     {
        //         return;
        //     }
        //
        //     if (string.IsNullOrWhiteSpace(playerName))
        //     {
        //         return;;
        //     }
        //
        //     // use Path.Combine to account for different OS's having different path separators
        //     string fullPath = Path.Combine(DataPath, playerName + ";" + playerID.Guid, FileName + Extension);
        //     string backupFilePath = fullPath + BackupExtension;
        //     try 
        //     {
        //         // create the directory the file will be written to if it doesn't already exist
        //         Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        //
        //         // serialize the C# game data object into Json
        //         //string dataToStore = JsonUtility.ToJson(data, true);
        //         string dataToStore = JsonConvert.SerializeObject(data, Formatting.Indented, JsonUtil.JsonSettingsForUnityObjects());
        //
        //         // optionally encrypt the data
        //         if (UseEncryption) 
        //         {
        //             dataToStore = EncryptDecrypt(dataToStore);
        //         }
        //
        //         // write the serialized data to the file
        //         using (FileStream stream = new FileStream(fullPath, FileMode.Create))
        //         {
        //             using (StreamWriter writer = new StreamWriter(stream)) 
        //             {
        //                 writer.Write(dataToStore);
        //             }
        //         }
        //
        //         // verify the newly saved file can be loaded successfully
        //         IGameData<T> verifiedGameData = Load<T>(playerID, playerName);
        //         // if the data can be verified, back it up
        //         if (verifiedGameData != null) 
        //         {
        //             File.Copy(fullPath, backupFilePath, true);
        //         }
        //         // otherwise, something went wrong and we should throw an exception
        //         else 
        //         {
        //             throw new Exception("Save file could not be verified and backup could not be created.");
        //         }
        //
        //     }
        //     catch (Exception e) 
        //     {
        //         Debug.LogError($"Error occured when trying to save data to file: {fullPath}\n{e.Message}");
        //     }
        //     
        // }

        // public virtual T Load<T>(SerializableGuid playerID, string playerName, bool allowRestoreFromBackup = true) where T : class, IGameData<T>
        // {
        //     // base case - if the profileId is null, return right away
        //     if (playerID.Guid == Guid.Empty) 
        //     {
        //         return default;
        //     }
        //
        //     if (string.IsNullOrWhiteSpace(playerName))
        //     {
        //         return default;
        //     }
        //
        //     // use Path.Combine to account for different OS's having different path separators
        //     string fullPath = Path.Combine(DataPath, playerName + ";" + playerID.Guid, FileName + Extension);
        //     T loadedData = default(T);
        //     if (File.Exists(fullPath))
        //     {
        //         try 
        //         {
        //             // load the serialized data from the file
        //             string dataToLoad = "";
        //             using (FileStream stream = new FileStream(fullPath, FileMode.Open))
        //             {
        //                 using (StreamReader reader = new StreamReader(stream))
        //                 {
        //                     dataToLoad = reader.ReadToEnd();
        //                 }
        //             }
        //
        //             // optionally decrypt the data
        //             if (UseEncryption) 
        //             {
        //                 dataToLoad = EncryptDecrypt(dataToLoad);
        //             }
        //
        //             // deserialize the data from Json back into the C# object
        //             //loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
        //             loadedData = JsonConvert.DeserializeObject<T>(dataToLoad, JsonUtil.JsonSettingsForUnityObjects());
        //         }
        //         catch (Exception e) 
        //         {
        //             // since we're calling Load(..) recursively, we need to account for the case where
        //             // the rollback succeeds, but data is still failing to load for some other reason,
        //             // which without this check may cause an infinite recursion loop.
        //             if (allowRestoreFromBackup) 
        //             {
        //                 Debug.LogWarning($"Failed to load data file. Attempting to roll back.\n{e.Message}");
        //                 bool rollbackSuccess = AttemptRollback(fullPath);
        //                 if (rollbackSuccess)
        //                 {
        //                     // try to load again recursively
        //                     loadedData = Load<T>(playerID, playerName, false);
        //                 }
        //             }
        //             // if we hit this else block, one possibility is that the backup file is also corrupt
        //             else 
        //             {
        //                 Debug.LogError($"Error occured when trying to load file at path: {fullPath} and backup did not work.\n {e.Message}");
        //             }
        //         }
        //     }
        //     return loadedData;        
        // }

        /// <summary>
        /// Deletes the saved data for the specified profile.
        /// </summary>
        /// Executes the Delete method.
        /// Handles the Delete functionality.
        /// </summary>
        public virtual void Delete(SerializableGuid playerID, string playerName) 
        {
            // base case - if the profileId is null, return right away
            if (playerID.Guid == Guid.Empty) 
            {
                return;
            }

            string fullPath = Path.Combine(DataPath, playerName + ";" + playerID.Guid, FileName + Extension);
            try 
            {
                // ensure the data file exists at this path before deleting the directory
                if (File.Exists(fullPath)) 
                {
                    // delete the profile folder and everything within it
                    Directory.Delete(Path.GetDirectoryName(fullPath), true);
                }
                else 
                {
                    Debug.LogWarning($"Tried to delete profile data, but data was not found at path: {fullPath}");
                }
            }
            catch (Exception e) 
            {
                Debug.LogError($"Failed to delete profile data for profileId: {playerID} at path: {fullPath}\n{e.Message}");
            }
        }

        public virtual (SerializableGuid playerID, T gameData) GetMostRecentlyUpdatedPlayer<T>() where T : class, IGameDataContent, new()
        {
            SerializableGuid mostRecentPlayerID = new(Guid.Empty);
            T mostRecentPlayerData = default(T);
            Dictionary<SerializableGuid, List<T>> profilesGameData = LoadAllProfiles<T>();
    
            foreach (KeyValuePair<SerializableGuid, List<T>> pair in profilesGameData) 
            {
                SerializableGuid playerID = pair.Key;
                List<T> gameDataList = pair.Value;

                // Skip this entry if the game data list is null or empty
                if (gameDataList == null || gameDataList.Count == 0) 
                {
                    continue;
                }

                // Assuming you want the most recently updated data from the list
                T mostRecentGameDataInList = gameDataList.FirstOrDefault(gd => gd?.ID?.Equals(playerID) == true);
                if (mostRecentGameDataInList == null) 
                {
                    continue;
                }

                // If this is the first data we've come across that exists, it's the most recent so far
                if (mostRecentPlayerID.Guid == Guid.Empty) 
                {
                    mostRecentPlayerID = playerID;
                    mostRecentPlayerData = mostRecentGameDataInList;
                }
                // Otherwise, compare to see which date is the most recent
                else 
                {
                    DateTime mostRecentDateTime = mostRecentPlayerData.LastUpdated;
                    DateTime newDateTime = mostRecentGameDataInList.LastUpdated;
            
                    // The greatest DateTime value is the most recent
                    if (newDateTime > mostRecentDateTime) 
                    {
                        mostRecentPlayerID = playerID;
                        mostRecentPlayerData = mostRecentGameDataInList;
                    }
                }
            }
    
            return (mostRecentPlayerID, mostRecentPlayerData);
        }


        public virtual Dictionary<SerializableGuid, List<T>> LoadAllProfiles<T>() where T : class, IGameDataContent, new()
        {
            Dictionary<SerializableGuid, List<T>> profileDictionary = new Dictionary<SerializableGuid, List<T>>();

            if(Directory.Exists(DataPath) == false)
            {
                Directory.CreateDirectory(DataPath);
            }
            // loop over all directory names in the data directory path
            IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(DataPath).EnumerateDirectories();
            foreach (DirectoryInfo dirInfo in dirInfos)
            {
                var fileName = dirInfo.Name.Split(';');
                string playerName = fileName[0];
                SerializableGuid playerID = new(Guid.Parse(fileName[1]));

                // defensive programming - check if the data file exists
                // if it doesn't, then this folder isn't a profile and should be skipped
                string fullPath = Path.Combine(DataPath, playerName + ";" + playerID.Guid, FileName + Extension);
                if (!File.Exists(fullPath))
                {
                    Debug.LogWarning($"Skipping directory when loading all profiles because it does not contain data: {playerID}");
                    continue;
                }

                // load the game data for this profile and put it in the dictionary
                List<T> profileDataList = Load<T>(playerID, playerName);
        
                // defensive programming - ensure the profile data isn't null or empty,
                // because if it is then something went wrong and we should let ourselves know
                if (profileDataList != null && profileDataList.Count > 0) 
                {
                    if (!profileDictionary.TryAdd(playerID, profileDataList))
                    {
                        Debug.LogError("Duplicate Profile ID when loading");                       
                    }
                }
                else 
                {
                    Debug.LogError($"Tried to load profile but something went wrong. ProfileId: {playerID}");
                }
            }

            return profileDictionary;
        }

        
        /// <summary>
        /// Encrypts/Decrypts the given data using a simple XOR cipher. 
        /// </summary>
        /// <param name="data">The data to encrypt/decrypt.</param>
        /// <returns>The encrypted/decrypted version of the data.</returns>
        public virtual string EncryptDecrypt(string data) 
        {
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++) 
            {
                modifiedData += (char) (data[i] ^ EncryptionCodeWord[i % EncryptionCodeWord.Length]);
            }
            return modifiedData;
        }

        /// <summary>
        /// Attempts to roll back to the backup file if it exists.
        /// </summary>
        /// <param name="fullPath">Path to main data file.</param>
        /// <returns>True if successfully rolled back, otherwise false.</returns>
        public virtual bool AttemptRollback(string fullPath) 
        {
            bool success = false;
            string backupFilePath = fullPath + BackupExtension;
            try 
            {
                // if the file exists, attempt to roll back to it by overwriting the original file
                if (File.Exists(backupFilePath))
                {
                    File.Copy(backupFilePath, fullPath, true);
                    success = true;
                    Debug.LogWarning($"Had to roll back to backup file at: {backupFilePath}");
                }
                // otherwise, we don't yet have a backup file - so there's nothing to roll back to
                else 
                {
                    throw new Exception("Tried to roll back, but no backup file exists to roll back to.");
                }
            }
            catch (Exception e) 
            {
                Debug.LogError($"Error occured when trying to roll back to backup file at: {backupFilePath}\n {e.Message}");
            }

            return success;
        }
    }


}
