using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CreatorButtonsMenu : MonoBehaviour
    {
        public Canvas Canvas;
        public Button ButtonPrefab;

        public void CreateButtonsMenuToLoadScenes(Action<string> loadNewScene)
        {
            var nameScenes = GetAllNamesScenes();
            var border = 10;
            var leftOffset = 100;
            var topOffset = 50;
            for (var i = 0; i < nameScenes.Count; i++)
            {
                CreateButton(
                    loadNewScene,
                    new Vector3(
                        leftOffset,
                        Canvas.GetComponent<RectTransform>().rect.height - (ButtonPrefab.GetComponent<RectTransform>().rect.height + border) * i - topOffset),
                    Path.GetFileName(nameScenes[i]));
            }
        }

        private List<string> GetAllNamesScenes()
        {
            return Directory.GetFiles(UnityConstants.PathToAssetBundles, "*.")
                .Where(x => Path.GetFileName(x).StartsWith(UnityConstants.SceneName))
                .ToList();
        }

        private void CreateButton(Action<string> loadNewScene, Vector3 position, string name)
        {
            var button = Instantiate(ButtonPrefab);
            button.transform.SetParent(Canvas.transform, false);
            button.transform.position = position;
            button.GetComponentInChildren<Text>().text = name;
            button.onClick.AddListener(() => loadNewScene(name));
        }
    }
}
