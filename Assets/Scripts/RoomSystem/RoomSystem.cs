using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.RoomSystem
{
    public class RoomSystem : MonoBehaviour
    {
        public static RoomSystem Instance { get; private set; }

        public Room CurrentRoom = Room.MainMenu;
        Room LastGameplayRoom = Room.Cantata;

        Maid DataPersistenceMaid = new();
        Maid SceneMaid = new();

        public event Action<Room> RoomChanged;

        void Awake()
        {
            Debug.Assert(Instance is null, "Can only have one instance of RoomSystem!");

            Instance = this;
        }

        private void OnEnable()
        {
            DataPersistenceMaid.GiveEvent(
                DataPersistenceManager.Instance,
                "onSaveTriggered",
                SaveData
            );
            DataPersistenceMaid.GiveEvent(
                DataPersistenceManager.Instance,
                "onLoadTriggered",
                LoadData
            );

            static void DisableInScene(Scene scene, LoadSceneMode scenemode)
            {
                // scene mode 4 is when its loaded via the play button in the editor
                if ((int)scenemode == 4)
                {
                    return;
                }

                Array.ForEach(
                    FindObjectsOfType<DisableInBuild>(),
                    (DisableInBuild obj) =>
                    {
                        obj.gameObject.SetActive(false);
                    }
                );
            }

            SceneManager.sceneLoaded += DisableInScene;

            SceneMaid.GiveTask(() => SceneManager.sceneLoaded -= DisableInScene);
        }

        private void OnDisable()
        {
            DataPersistenceMaid.Cleanup();
        }

        static string[] RoomAsScenes(Room room)
        {
            return room switch
            {
                Room.MainMenu => new string[] { "MainMenu" },
                Room.Vault => new string[] { "Vault" }, // TODO
                Room.CityHall => new string[] { "CityHall" },
                Room.Cantata
                    => new string[]
                    {
                        "CantataBase",
                        "Central",
                        "MediaDistrict",
                        "ElectricDistrict",
                        "Uptown",
                    },
                Room.AmPower => new string[] { "AmPower" },
                Room.Channel440 => new string[] { "Channel440" },
                Room.YCorp => new string[] { "YCorp" }, // TODO
                _ => throw new NotImplementedException($"Unimplemented room loading {room}"),
            };
        }

        public static void LoadLastGameplayRoom()
        {
            LoadRoom(Instance.LastGameplayRoom, "");
        }

        public static void LoadRoom(Room toLoad, InitialRoomPoint entrance)
        {
            // TODO get transform for entrance
            LoadRoom(
                toLoad,
                () =>
                {
                    return null;
                }
            );
        }

        public static void LoadRoom(Room toLoad, string spawnId)
        {
            // TODO get transform for spawn
            LoadRoom(
                toLoad,
                () =>
                {
                    return null;
                }
            );
        }

        public static void LoadRoom(Room toLoad, Func<Transform> getCharacterInitialTransform)
        {
            Instance.StartCoroutine(LoadScenes(toLoad, getCharacterInitialTransform));
            Instance.CurrentRoom = toLoad;
        }

        private static IEnumerator LoadScenes(
            Room toLoad,
            Func<Transform> getCharacterInitialTransform
        )
        {
            //TODO KILL THIS GUY
            AudioSystem.Instance.Songs.Clear();
            string[] scenesToLoad = RoomAsScenes(toLoad);

            // TODO refactor this guy
            // load the loading scene
            var asyncLoadLevel = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Single);
            while (!asyncLoadLevel.isDone)
            {
                //Debug.Log("Loading loading screen");
                yield return null;
            }

            // load each scene and track its progress
            AsyncOperation[] additiveScenesLoading = new AsyncOperation[scenesToLoad.Length];
            for (int i = 0; i < scenesToLoad.Length; i++)
            {
                additiveScenesLoading[i] = SceneManager.LoadSceneAsync(
                    scenesToLoad[i],
                    LoadSceneMode.Additive
                );
            }

            // wait until all scenes loaded
            while (!Array.TrueForAll(additiveScenesLoading, (asyncOp) => asyncOp.isDone))
            {
                //Debug.Log("In loading screen");
                yield return null;
            }

            SceneManager.UnloadSceneAsync("Loading");

            Instance.RoomChanged?.Invoke(toLoad);

            // TODO position character
        }

        public void LoadData()
        {
            var loadedData = DataPersistenceManager.LoadData<SerializableRoomData>(
                "LastGameplayRoom"
            );
            LastGameplayRoom = loadedData.LastGameplayRoom;
        }

        public void SaveData()
        {
            DataPersistenceManager.SaveData(new SerializableRoomData(LastGameplayRoom));
        }

        [Serializable]
        internal class SerializableRoomData
        {
            public Room LastGameplayRoom = new();

            public SerializableRoomData(Room lastGameplayRoom)
            {
                LastGameplayRoom = lastGameplayRoom;
            }
        }
    }
}
