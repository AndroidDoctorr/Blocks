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
        private readonly Dictionary<string, Block> _blocks = new Dictionary<string, Block>();

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
