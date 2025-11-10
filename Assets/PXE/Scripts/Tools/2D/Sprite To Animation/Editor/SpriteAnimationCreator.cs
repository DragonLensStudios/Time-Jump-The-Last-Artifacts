using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace PXE.Tools._2D.Sprite_To_Animation.Editor
{
    public class SpriteAnimationCreator : EditorWindow
    {
        private Texture2D spriteTexture;
        private SpriteAnimationTemplate animationTemplate;
        private string prefabName;
        private string outputPath;
        private Sprite[] sprites;
        private Dictionary<int, List<Sprite>> spritesByRow;

        [MenuItem("PXE/Tools/Sprite Animation Creator")]
        public static void ShowWindow()
        {
            GetWindow<SpriteAnimationCreator>("Sprite Animation Creator");
        }

        void OnGUI()
        {
            // Display a label for the sprite animation creator
            GUILayout.Label("Sprite Animation Creator", EditorStyles.boldLabel);
    
            // Allow the user to select a sprite texture
            spriteTexture = EditorGUILayout.ObjectField("Sprite Texture", spriteTexture, typeof(Texture2D), false) as Texture2D;
    
            // Allow the user to select an animation template
            animationTemplate = EditorGUILayout.ObjectField("Animation Template", animationTemplate, typeof(SpriteAnimationTemplate), false) as SpriteAnimationTemplate;
    
            // Allow the user to enter a prefab name
            prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);
    
            // Allow the user to enter an output path
            EditorGUILayout.LabelField("Output Path");
            GUILayout.BeginHorizontal();
            outputPath = EditorGUILayout.TextField(outputPath);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string fullPath = EditorUtility.OpenFolderPanel("Select Output Path", Application.dataPath, "");
                if (fullPath.StartsWith(Application.dataPath))
                {
                    outputPath = fullPath.Substring(Application.dataPath.Length + 1);
                }
                else
                {
                    Debug.LogError("Please select a folder within the Assets folder!");
                }
            }
            GUILayout.EndHorizontal();

            // Check if the "Generate" button is clicked
            if (GUILayout.Button("Generate"))
            {
                // Check if any of the required fields are empty
                if (spriteTexture == null || animationTemplate == null || string.IsNullOrEmpty(prefabName) || string.IsNullOrEmpty(outputPath))
                {
                    // Display an error message and return
                    Debug.LogError("All fields must be filled!");
                    return;
                }
        
                // Generate animations and prefab
                GenerateAnimationsAndPrefab();
            }
        }




        private void GenerateAnimationsAndPrefab()
        {
            // Slice the sprite texture into rows
            spritesByRow = SliceSprite(spriteTexture, animationTemplate.gridSize);
             // Find the maximum size of a row
            var maxRowSize = spritesByRow.Values.Max(rowSprites => rowSprites.Count);
             // Convert Dictionary to a single array and sort sprites by name
            sprites = spritesByRow.Values.SelectMany(row => row).OrderBy(s => s.name).ToArray();
             // Create the full path for the output folder
            var fullPath = Path.Combine("Assets", outputPath);
             // Check if the folder already exists, if not create it
            if (!AssetDatabase.IsValidFolder(fullPath))
            {
                var subFolders = outputPath.Split('/');
                var parentFolder = "Assets";
                for(var i = 0; i < subFolders.Length; i++) {
                    string newFolderPath = Path.Combine(parentFolder, subFolders[i]);
                    if(!AssetDatabase.IsValidFolder(newFolderPath)) {
                        AssetDatabase.CreateFolder(parentFolder, subFolders[i]);
                    }
                    parentFolder = newFolderPath;
                }
            }
             // Generate animations and animator controller
            var animatorController = GenerateAnimationsAndAnimator(sprites, animationTemplate.animationDetails, maxRowSize, fullPath);
             // Create the prefab
            CreatePrefab(animatorController, fullPath);
        }

        private Dictionary<int, List<Sprite>> SliceSprite(Texture2D texture, Vector2Int gridSize)
        {
            // Get the asset path and importer for the texture
            var path = AssetDatabase.GetAssetPath(texture);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            // Make the texture readable and update the asset import settings
            importer.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            // Set the sprite import mode, pixels per unit, filter mode, and texture compression
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = gridSize.x; // Set the Pixels Per Unit to the size of the cells
            importer.filterMode = FilterMode.Point; // Set filter mode to Point (no filter)
            importer.textureCompression = TextureImporterCompression.Uncompressed; // Set texture format to RGBA 32 bit
            // Calculate the cell size, number of columns, and number of rows
            var cellSizeX = gridSize.x; // Cell size in pixels (width)
            var cellSizeY = gridSize.y; // Cell size in pixels (height)
            var cols = texture.width / cellSizeX;
            var rows = texture.height / cellSizeY;
            // Create a dictionary to store the sprites by row
            var spritesByRow = new Dictionary<int, List<SpriteMetaData>>();
            // Iterate through each row
            for (var y = 0; y < rows; y++)
            {
                var spritesInRow = new List<SpriteMetaData>();
                // Iterate through each column
                for (var x = 0; x < cols; x++)
                {
                    // Create a rectangle representing the current cell
                    Rect cellRect = new Rect(x * cellSizeX, y * cellSizeY, cellSizeX, cellSizeY);
                    // Check if the cell is empty
                    if (!IsCellEmpty(texture, cellRect))
                    {
                        // Create a new sprite metadata
                        var sprite = new SpriteMetaData
                        {
                            alignment = (int)SpriteAlignment.Center,
                            border = new Vector4(),
                            name = $"{texture.name}_{rows - 1 - y}_{x}", // Reverse the y coordinate for top-left origin
                            pivot = new Vector2(0.5f, 0.5f),
                            rect = cellRect
                        };
                        // Add the sprite metadata to the list
                        spritesInRow.Add(sprite);
                    }
                }
                // Check if there are any sprites in the row
                if (spritesInRow.Count > 0)
                {
                    // Add the row to the dictionary, reversing the y coordinate for top-left origin
                    spritesByRow.Add(rows - 1 - y, spritesInRow);
                }
            }
            // Set the spritesheet in the importer and save the changes
            importer.spritesheet = spritesByRow.Values.SelectMany(row => row).ToArray();
            importer.SaveAndReimport();
            // Load all sprite assets at the path
            var spriteAssets = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>();
            // Sort the sprite assets by name
            var sprites = spriteAssets.OrderBy(x => x.name).ToArray();
            // Create a dictionary to store the sprites by row as assets
            var spritesByRowAssets = new Dictionary<int, List<Sprite>>();
            // Iterate through each row in the sprites by row dictionary
            foreach (var key in spritesByRow.Keys)
            {
                // Create a new list to store the sprites in the row
                spritesByRowAssets[key] = new List<Sprite>();
                // Iterate through each column in the row
                for (int i = 0; i < cols; i++)
                {
                    // Create the sprite name
                    var spriteName = $"{texture.name}_{key}_{i}";
                    // Find the sprite asset with the matching name
                    var sprite = spriteAssets.FirstOrDefault(s => s.name == spriteName);
                    // Check if a matching sprite was found
                    if (sprite != null)
                    {
                        // Add the sprite to the list
                        spritesByRowAssets[key].Add(sprite);
                    }
                }
            }
            // Output debug information for each row in the sprites by row assets dictionary
            foreach (var row in spritesByRowAssets)
            {
                var spriteNames = string.Join(", ", row.Value.Select(s => s.name));
                Debug.Log($"Row: {row.Key}, Sprites: {spriteNames}");
            }
            // Return the sprites by row assets dictionary
            return spritesByRowAssets;
        }

        // This method checks if a cell in a texture is empty or not
        private bool IsCellEmpty(Texture2D texture, Rect cellRect)
        {
            // Iterate through each pixel in the cell
            for (int y = (int)cellRect.yMin; y < (int)cellRect.yMax; y++)
            {
                for (int x = (int)cellRect.xMin; x < (int)cellRect.xMax; x++)
                {
                    // If the pixel is not fully transparent, the cell is not empty
                    if (texture.GetPixel(x, y).a != 0)
                    {
                        return false;
                    }
                }
            }
             // If all pixels in the cell are fully transparent, the cell is empty
            return true;
        }

        private AnimatorController GenerateAnimationsAndAnimator(Sprite[] sprites, List<AnimationDetail> animationDetails, int rowSize, string fullPath)
        {
            // Create animator controller
            var controller = AnimatorController.CreateAnimatorControllerAtPath($"{fullPath}/{spriteTexture.name}_Animator.controller");
            var rootStateMachine = controller.layers[0].stateMachine;
             // Add animator parameters
            controller.AddParameter("move_x", AnimatorControllerParameterType.Float);
            controller.AddParameter("move_y", AnimatorControllerParameterType.Float);
            controller.AddParameter("isMoving", AnimatorControllerParameterType.Bool);
             // Create movement blend tree
            var movementBlendTree = new BlendTree();
            AssetDatabase.AddObjectToAsset(movementBlendTree, controller);
            movementBlendTree.blendType = BlendTreeType.SimpleDirectional2D;
            movementBlendTree.blendParameter = "move_x";
            movementBlendTree.blendParameterY = "move_y";
             // Create idle blend tree
            var idleBlendTree = new BlendTree();
            AssetDatabase.AddObjectToAsset(idleBlendTree, controller);
            idleBlendTree.blendType = BlendTreeType.SimpleDirectional2D;
            idleBlendTree.blendParameter = "move_x";
            idleBlendTree.blendParameterY = "move_y";
             // Create idle and movement states
            var idleState = rootStateMachine.AddState("Idle");
            var movementState = rootStateMachine.AddState("Movement");
             // Generate clips and add to blend trees
            foreach (var animDetail in animationDetails)
            {
                var clip = GenerateClip(sprites, animDetail, rowSize, fullPath);
                var motion = new ChildMotion { motion = clip, position = animDetail.direction, timeScale = 1 };
                 if (animDetail.animationType == AnimationType.Idle)
                {
                    idleBlendTree.children = idleBlendTree.children.Append(motion).ToArray();
                }
                else if (animDetail.animationType == AnimationType.Movement)
                {
                    movementBlendTree.children = movementBlendTree.children.Append(motion).ToArray();
                }
            }
             // Assign blend trees to states
            idleState.motion = idleBlendTree;
            movementState.motion = movementBlendTree;
             // Add transitions between states
            var toMovement = idleState.AddTransition(movementState);
            toMovement.AddCondition(AnimatorConditionMode.If, 0, "isMoving");
             var toIdle = movementState.AddTransition(idleState);
            toIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "isMoving");
             // Check if idle blend tree has any animations
             if (idleBlendTree.children.Length != 0) return controller;
             Debug.LogError("No AnimationDetail with AnimationType.Idle found!");
            return null;
        }


        private void CreatePrefab(AnimatorController animatorController, string fullPath)
        {
            var obj = new GameObject(prefabName);
            var renderer = obj.AddComponent<SpriteRenderer>();
            var animator = obj.AddComponent<Animator>();

            animator.runtimeAnimatorController = animatorController;
            renderer.sprite = sprites[0];

            PrefabUtility.SaveAsPrefabAsset(obj, $"{fullPath}/{prefabName}.prefab");
            DestroyImmediate(obj);
        }

        private AnimationClip GenerateClip(Sprite[] sprites, AnimationDetail detail, int rowSize, string fullPath)
        {
            // Compute start and end index
            var startIndex = (detail.row - 1) * rowSize + (detail.startColumn - 1);
            var endIndex = (detail.row - 1) * rowSize + (detail.endColumn - 1);
            Debug.Log($"Start Index: {startIndex}, End Index: {endIndex}");
            var clip = new AnimationClip { frameRate = detail.frameRate }; // Set the frame rate according to the sample rate
            var spriteBinding = EditorCurveBinding.PPtrCurve(string.Empty, typeof(SpriteRenderer), "m_Sprite");
            var frameCount = endIndex - startIndex + 1;
            var keyframes = new ObjectReferenceKeyframe[frameCount];
            Debug.Log($"startIndex: {startIndex}, endIndex: {endIndex}, frameCount: {frameCount}, sprites.Length: {sprites.Length}");
            for (int i = 0; i < frameCount; i++)
            {
                // Compute the sprite index in the sprites array
                var spriteIndex = startIndex + i;
                if (spriteIndex >= sprites.Length)
                {
                    Debug.LogError($"Index out of bounds: {spriteIndex}");
                    return null;
                }

                // The time of each keyframe is determined by the frame duration.
                float time = (float)i / detail.frameRate; 
                keyframes[i] = new ObjectReferenceKeyframe { time = time, value = sprites[spriteIndex] };
            }
            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes); 
            // Set the clip to loop if loop is set to true.
            var settings = new AnimationClipSettings
            {
               loopTime = detail.loop
            };
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            clip.frameRate = detail.frameRate;
            var assetPath = $"{fullPath}/{spriteTexture.name}_{detail.animationName}.anim";
            var assetDirectory = Path.GetDirectoryName(assetPath);
            if (!AssetDatabase.IsValidFolder(assetDirectory))
            {
                var subFolders = assetDirectory.Split(new string[] { "/" }, StringSplitOptions.None);
                var parentFolder = "Assets";
                for (int i = 0; i < subFolders.Length; i++)
                {
                    var newFolderPath = Path.Combine(parentFolder, subFolders[i]);
                    if (!AssetDatabase.IsValidFolder(newFolderPath))
                    {
                        AssetDatabase.CreateFolder(parentFolder, subFolders[i]);
                    }
                    parentFolder = newFolderPath;
                }
            }
            AssetDatabase.CreateAsset(clip, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return clip;
        }
    }
}
