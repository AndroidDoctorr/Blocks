using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class BlocksRepo
    {
        private static Dictionary<string, Block> _blocks = new Dictionary<string, Block>();

        public bool HasOccupantAt(string coordString)
        {
            return _blocks.ContainsKey(coordString);
        }
        public void PlaceBlock(
            GameObject go,
            string shape, string material,
            int x, int y, int z,
            float wRot, float xRot, float yRot, float zRot
        )
        {
            string coordString = $"{x},{y},{z}";
            if (HasOccupantAt(coordString))
            {
                Debug.LogWarning("Tried to place a block at an existing spot!");
                return;
            }
            Block block = new Block(
                go, shape, material,
                x, y, z,
                wRot, xRot, yRot, zRot
            );
            _blocks.Add(coordString, block);
        }
        public void RemoveBlock(string coordString)
        {
            if (!_blocks.ContainsKey(coordString))
            {
                Debug.LogWarning($"Attempted to remove non-existent block at {coordString}");
                return;
            }
            Block block = _blocks[coordString];
            block.Go.transform.parent = null;
            GameObject.Destroy(block.Go);
            _blocks.Remove(coordString);
        }

        public void ClearPlayArea()
        {
            foreach (KeyValuePair<string, Block> kvp in _blocks)
            {
                string key = kvp.Key;
                RemoveBlock(key);
            }
        }

        public string RenderPlayArea()
        {
            List<string> blockStrings = new List<string>();
            foreach (KeyValuePair<string, Block> kvp in _blocks)
            {
                // Render the block as a string and append to total
                Block block = kvp.Value;
                blockStrings.Add(block.ToString());
            }
            // Return the entire string encoded as Base 64
            string playAreaString = string.Join(";", blockStrings.ToArray());
            byte[] stringBytes = Encoding.UTF8.GetBytes(playAreaString);
            string encodedString = Convert.ToBase64String(stringBytes);
            return encodedString;
        }

        public static void LoadPlayArea(string encodedString)
        {
            if (string.IsNullOrEmpty(encodedString)) return;
            // Decode string into block strings
            byte[] stringBytes = Convert.FromBase64String(encodedString);
            string playAreaString = Encoding.UTF8.GetString(stringBytes);
            string[] blockStrings = playAreaString.Split(';');
            // Clear dictionary
            _blocks = new Dictionary<string, Block>();
            // Create blocks from strings
            foreach (string blockString in blockStrings)
            {
                PlaceBlockFromString(blockString);
            }
        }

        private static void PlaceBlockFromString(string blockString)
        {
            string[] pieces = blockString.Split('_');
            if (pieces.Length < 4)
            {
                Debug.LogError($"Invalid block string: {blockString}");
                return;
            }

            string shapeString = pieces[0];
            string materialString = pieces[1];
            string coordString = pieces[2];
            string rotationString = pieces[3];

            // Get shape and material from dictionaries in Selector
            GameObject shape = BlockSelector.ShapesReference[shapeString];
            Material material = null;
            if (BlockSelector.MaterialsReference.ContainsKey(materialString))
                material = BlockSelector.MaterialsReference[materialString];
            // Get coordinates and position
            int[] coords = coordString.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
            float unit = BlockPlacer._unit;
            Vector3 position = new Vector3(coords[0] * unit, coords[1] * unit, coords[2] * unit);
            // Get orientation
            float[] rotations = rotationString.Split(',').Select(s => (float) Convert.ToDouble(s)).ToArray();
            Quaternion rotation = new Quaternion(rotations[1], rotations[2], rotations[3], rotations[0]);

            // Instantiate the game object and apply material if necessary
            var go = GameObject.Instantiate(shape, position, rotation);
            if (material != null)
            {
                Renderer renderer = go.GetComponentInChildren<Renderer>();
                renderer.material = material;
            }
            
            // Add to dictionary
            _blocks.Add(coordString, new Block(go,
                    blockString, materialString,
                    coords[0], coords[1], coords[2],
                    rotation.w, rotation.x, rotation.y, rotation.z
                ));
        }

        private class Block
        {
            public Block(GameObject go, string shape, string material,
                int x, int y, int z,
                float wRot, float xRot, float yRot, float zRot)
            {
                Go = go;
                _shape = shape;
                _material = material;
                _x = x;
                _y = y;
                _z = z;
                _wRot = wRot;
                _xRot = xRot;
                _yRot = yRot;
                _zRot = zRot;
            }

            public GameObject Go;
            private string _shape;
            private string _material;

            private float _x;
            private float _y;
            private float _z;
            
            private float _wRot;
            private float _xRot;
            private float _yRot;
            private float _zRot;

            public override string ToString()
            {
                return $"{_shape}_{_material}_{_x},{_y},{_z}_{_wRot},{_xRot},{_yRot},{_zRot}";
            }
        }
    }
}
