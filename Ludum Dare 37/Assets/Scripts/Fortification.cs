﻿using UnityEngine;

namespace Assets.Scripts
{
    public class Fortification
    {
        public enum Type
        {
            NONE = 0,
            TOWER1 = 1,
            TOWER2 = 2,
            TOWER3 = 3,
            TOWER4 = 4,
            TOWER5 = 5,
        };
        private Type _type = Type.NONE;
        public new Type GetType() { return _type; }
        private BaseFortification _fort = null;
        private Vector3 _pos;
        public Vector3 GetPos() { return _pos; }
        
        public bool IsSet()
        {
            return (_type != Type.NONE);
        }

        public Fortification(Type type, Vector3 pos)
        {
            _pos = pos;
            SetType(type);
        }

        public void SetType(Type type)
        {
            if (!IsSet())
            {
                _fort = GetPrefab(type);
                _fort.gameObject.transform.position = _pos;
                _fort.PlayAudio();
                _type = type;
            }
            else
            {
                Debug.Log("Fortification Already Set.");
            }
        }

        public void RemoveFortification()
        {
            if (IsSet())
            {
                if (_fort != null)
                {
                    GameObject.Destroy(_fort.gameObject);
                }
                _type = Type.NONE;
            }
        }
        
        //@TEMP: Until we get SpriteRenderers going...
        public Color GetColor()
        {
            //@TODO: Draw the legit tower sprite er whatever.
            switch (_type)
            {
                case Type.TOWER1:
                    return Color.red;
                case Type.TOWER2:
                    return Color.cyan;
                case Type.TOWER3:
                    return Color.green;
                case Type.TOWER4:
                    return Color.yellow;
                case Type.TOWER5:
                    return Color.blue;
                default:
                    return Color.black;
            }
        }

        //@NOTE: Used to retrieve Prefab of the logic contained within the Fortification - e.g., Elf Sniper's game code, Mine behavior, etc etc.
        static public BaseFortification GetPrefab(Type type)
        {
            GameObject go = null;

            switch (type)
            {
                case Type.TOWER1:
                    go = (GameObject)GameObject.Instantiate(Resources.Load("spirit_gen"));
                    break;
                case Type.TOWER2:
                    go = (GameObject)GameObject.Instantiate(Resources.Load("coal_elf"));
                    break;
                case Type.TOWER3:
                    go = (GameObject)GameObject.Instantiate(Resources.Load("mine"));
                    break;
                case Type.TOWER4:
                    go = (GameObject)GameObject.Instantiate(Resources.Load("elf_sniper"));
                    break;
                case Type.TOWER5:
                    go = (GameObject)GameObject.Instantiate(Resources.Load("candy_cane"));
                    break;
                default:
                    Debug.LogError("ERROR!: We don't recognize your authority heeeyah. This type is fucked.");
                    return null;
            }

            return go.GetComponent<BaseFortification>();
        }

        static public BaseFortification GetPreviewPrefab(Type type)
        {
            BaseFortification preview = GetPrefab(type);
            preview.SetPreview();
            return preview;
        }

        public bool IsAttackable()
        {
            return (IsSet() && _fort != null && _fort.IsAttackable());
        }

        public void TakeDamage(int damage)
        {
            if (IsAttackable())
            {
                if (_fort.TakeDamage(damage))
                {
                    //@TODO: Maybe signal destruction to allow for animation state, etc.
                    RemoveFortification();
                }
            }
        }
    }
}
