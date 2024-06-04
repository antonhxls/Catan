using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using Assets;
using UnityEditor;
using Unity.VisualScripting;

namespace Assets
{
    public enum FieldType
    {
        Desert,
        Ore,
        Brick,
        Sheep,
        Wood,
        Wheat
    }

    public class Field
    {

        #region Properties

        /// <summary>
        /// GameObject of the hexagon tile
        /// </summary>
        public GameObject GameObject { get; set; }

        /// <summary>
        /// FieldType
        /// </summary>
        public FieldType FieldType { get; set; }

        /// <summary>
        /// Number of this field 
        /// </summary>
        public int FieldNumber { get; set; }

        /// <summary>
        /// Number that has to be rolled in order to hand out the resources of this field
        /// </summary>
        public int DiceNumber { get; set; }

        public GameObject DiceNumberGameObject { get; set; }

        /// <summary>
        /// Returns true if the Robber is placed on this field right now
        /// </summary>
        public bool IsRobberPlacedOnThisField { get; set; }

        #region Streets

        public int LeftStreetPlacementNumber { get; set; }

        public int TopLeftStreetPlacementNumber { get; set; }

        public int TopRightStreetPlacementNumber { get; set; }

        public int RightStreetPlacementNumber { get; set; }

        public int BottomRightStreetPlacementNumber { get; set; }

        public int BottomLeftStreetPlacementNumber { get; set; }

        /// <summary>
        /// GameObject of the street placed on the left edge of this field
        /// </summary>
        public GameObject LeftStreetGameObject { get; set; }

        /// <summary>
        /// GameObject of the street placed on the top left edge of this field
        /// </summary>
        public GameObject TopLeftStreetGameObject { get; set; }

        /// <summary>
        /// GameObject of the street placed on the top right edge of this field
        /// </summary>
        public GameObject TopRightStreetGameObject { get; set; }

        /// <summary>
        /// GameObject of the street placed on the right edge of this field
        /// </summary>
        public GameObject RightStreetGameObject { get; set; }

        /// <summary>
        /// GameObject of the street placed on the bottom right edge of this field
        /// </summary>
        public GameObject BottomRightStreetGameObject { get; set; }

        /// <summary>
        /// GameObject of the street placed on the bottom left edge of this field
        /// </summary>
        public GameObject BottomLeftStreetGameObject { get; set; }

        #endregion Streets

        #region Buildings

        public int TopBuildingPlacementNumber { get; set; }

        public int TopRightBuildingPlacementNumber { get; set; }

        public int BottomRightBuildingPlacementNumber { get; set; }

        public int BottomBuildingPlacementNumber { get; set; }

        public int BottomLeftBuildingPlacementNumber { get; set; }

        public int TopLeftBuildingPlacementNumber { get; set; }

        /// <summary>
        /// GameObject of the building placed on the top of this field
        /// </summary>
        public GameObject TopBuildingGameObject { get; set; }

        /// <summary>
        /// GameObject of the building placed on the top right of this field
        /// </summary>
        public GameObject TopRightBuildingGameObject { get; set; }

        /// <summary>
        /// GameObject of the building placed on the bottom right of this field
        /// </summary>
        public GameObject BottomRightBuildingGameObject { get; set; }

        /// <summary>
        /// GameObject of the building placed on the bottom of this field
        /// </summary>
        public GameObject BottomBuildingGameObject { get; set; }

        /// <summary>
        /// GameObject of the building placed on the bottom left of this field
        /// </summary>
        public GameObject BottomLeftBuildingGameObject { get; set; }

        /// <summary>
        /// GameObject of the building placed on the top left of this field
        /// </summary>
        public GameObject TopLeftBuildingGameObject { get; set; }

        #endregion Buildings

        #endregion Properties

        #region Methods

        public void SetFieldType(FieldType fieldType)
        {
            //Set FieldType
            this.FieldType = fieldType;

            //Get material from fieldtype
            Material newMaterial = null;

            switch (fieldType)
            {
                case FieldType.Desert:
                    newMaterial = Game.DesertMaterial;
                    break;
                case FieldType.Ore:
                    newMaterial = Game.OreMaterial;
                    break;
                case FieldType.Brick:
                    newMaterial = Game.BrickMaterial;
                    break;
                case FieldType.Sheep:
                    newMaterial = Game.SheepMaterial;
                    break;
                case FieldType.Wheat:
                    newMaterial = Game.WheatMaterial;
                    break;
                case FieldType.Wood:
                    newMaterial = Game.WoodMaterial;
                    break;
            }

            //Set new material
            this.GameObject.GetComponent<MeshRenderer>().material = newMaterial;
        }

        public override string ToString()
        {
            return $"Field{this.FieldNumber}: {this.GameObject.name} with FieldType: {this.FieldType} and DiceNumber: {this.DiceNumber} and Position:{this.GameObject.transform.position}";
        }

        public void InitializeBuildingsAndStreets()
        {
            switch (this.FieldNumber)
            {
                case 0:
                    this.TopBuildingPlacementNumber = 52;
                    this.TopRightBuildingPlacementNumber = 51;
                    this.BottomRightBuildingPlacementNumber = 50;
                    this.BottomBuildingPlacementNumber = 49;
                    this.BottomLeftBuildingPlacementNumber = 0;
                    this.TopLeftBuildingPlacementNumber = 53;

                    this.TopLeftStreetPlacementNumber = 70;
                    this.TopRightStreetPlacementNumber = 69;
                    this.RightStreetPlacementNumber = 68;
                    this.BottomRightStreetPlacementNumber = 67;
                    this.BottomLeftStreetPlacementNumber = 0;
                    this.LeftStreetPlacementNumber = 71;
                    break;
                case 1:
                    this.TopBuildingPlacementNumber = 47;
                    this.TopRightBuildingPlacementNumber = 45;
                    this.BottomRightBuildingPlacementNumber = 46;
                    this.BottomBuildingPlacementNumber = 48;
                    this.BottomLeftBuildingPlacementNumber = 50;
                    this.TopLeftBuildingPlacementNumber = 51;

                    this.TopLeftStreetPlacementNumber = 66;
                    this.TopRightStreetPlacementNumber = 64;
                    this.RightStreetPlacementNumber = 63;
                    this.BottomRightStreetPlacementNumber = 62;
                    this.BottomLeftStreetPlacementNumber = 65;
                    this.LeftStreetPlacementNumber = 68;
                    break;
                case 2:
                    this.TopBuildingPlacementNumber = 44;
                    this.TopRightBuildingPlacementNumber = 43;
                    this.BottomRightBuildingPlacementNumber = 42;
                    this.BottomBuildingPlacementNumber = 41;
                    this.BottomLeftBuildingPlacementNumber = 46;
                    this.TopLeftBuildingPlacementNumber = 45;

                    this.TopLeftStreetPlacementNumber = 61;
                    this.TopRightStreetPlacementNumber = 60;
                    this.RightStreetPlacementNumber = 57;
                    this.BottomRightStreetPlacementNumber = 58;
                    this.BottomLeftStreetPlacementNumber = 59;
                    this.LeftStreetPlacementNumber = 63;
                    break;
                case 3:
                    this.TopBuildingPlacementNumber = 37;
                    this.TopRightBuildingPlacementNumber = 40;
                    this.BottomRightBuildingPlacementNumber = 52;
                    this.BottomBuildingPlacementNumber = 53;
                    this.BottomLeftBuildingPlacementNumber = 39;
                    this.TopLeftBuildingPlacementNumber = 38;

                    this.TopLeftStreetPlacementNumber = 54;
                    this.TopRightStreetPlacementNumber = 53;
                    this.RightStreetPlacementNumber = 52;
                    this.BottomRightStreetPlacementNumber = 70;
                    this.BottomLeftStreetPlacementNumber = 56;
                    this.LeftStreetPlacementNumber = 55;
                    break;
                case 4:
                    this.TopBuildingPlacementNumber = 36;
                    this.TopRightBuildingPlacementNumber = 35;
                    this.BottomRightBuildingPlacementNumber = 47;
                    this.BottomBuildingPlacementNumber = 51;
                    this.BottomLeftBuildingPlacementNumber = 52;
                    this.TopLeftBuildingPlacementNumber = 40;

                    this.TopLeftStreetPlacementNumber = 51;
                    this.TopRightStreetPlacementNumber = 50;
                    this.RightStreetPlacementNumber = 49;
                    this.BottomRightStreetPlacementNumber = 66;
                    this.BottomLeftStreetPlacementNumber = 69;
                    this.LeftStreetPlacementNumber = 52;
                    break;
                case 5:
                    this.TopBuildingPlacementNumber = 8;
                    this.TopRightBuildingPlacementNumber = 7;
                    this.BottomRightBuildingPlacementNumber = 44;
                    this.BottomBuildingPlacementNumber = 45;
                    this.BottomLeftBuildingPlacementNumber = 47;
                    this.TopLeftBuildingPlacementNumber = 35;

                    this.TopLeftStreetPlacementNumber = 48;
                    this.TopRightStreetPlacementNumber = 47;
                    this.RightStreetPlacementNumber = 46;
                    this.BottomRightStreetPlacementNumber = 61;
                    this.BottomLeftStreetPlacementNumber = 64;
                    this.LeftStreetPlacementNumber = 49;
                    break;
                case 6:
                    this.TopBuildingPlacementNumber = 6;
                    this.TopRightBuildingPlacementNumber = 5;
                    this.BottomRightBuildingPlacementNumber = 4;
                    this.BottomBuildingPlacementNumber = 43;
                    this.BottomLeftBuildingPlacementNumber = 44;
                    this.TopLeftBuildingPlacementNumber = 7;

                    this.TopLeftStreetPlacementNumber = 45;
                    this.TopRightStreetPlacementNumber = 44;
                    this.RightStreetPlacementNumber = 43;
                    this.BottomRightStreetPlacementNumber = 42;
                    this.BottomLeftStreetPlacementNumber = 60;
                    this.LeftStreetPlacementNumber = 46;
                    break;
                case 7:
                    this.TopBuildingPlacementNumber = 34;
                    this.TopRightBuildingPlacementNumber = 3;
                    this.BottomRightBuildingPlacementNumber = 37;
                    this.BottomBuildingPlacementNumber = 38;
                    this.BottomLeftBuildingPlacementNumber = 2;
                    this.TopLeftBuildingPlacementNumber = 1;

                    this.TopLeftStreetPlacementNumber = 39;
                    this.TopRightStreetPlacementNumber = 38;
                    this.RightStreetPlacementNumber = 37;
                    this.BottomRightStreetPlacementNumber = 54;
                    this.BottomLeftStreetPlacementNumber = 41;
                    this.LeftStreetPlacementNumber = 40;
                    break;
                case 8:
                    this.TopBuildingPlacementNumber = 33;
                    this.TopRightBuildingPlacementNumber = 32;
                    this.BottomRightBuildingPlacementNumber = 26;
                    this.BottomBuildingPlacementNumber = 40;
                    this.BottomLeftBuildingPlacementNumber = 37;
                    this.TopLeftBuildingPlacementNumber = 3;

                    this.TopLeftStreetPlacementNumber = 36;
                    this.TopRightStreetPlacementNumber = 35;
                    this.RightStreetPlacementNumber = 34;
                    this.BottomRightStreetPlacementNumber = 51;
                    this.BottomLeftStreetPlacementNumber = 53;
                    this.LeftStreetPlacementNumber = 37;
                    break;
                case 9:
                    this.TopBuildingPlacementNumber = 31;
                    this.TopRightBuildingPlacementNumber = 30;
                    this.BottomRightBuildingPlacementNumber = 8;
                    this.BottomBuildingPlacementNumber = 35;
                    this.BottomLeftBuildingPlacementNumber = 36;
                    this.TopLeftBuildingPlacementNumber = 32;

                    this.TopLeftStreetPlacementNumber = 15;
                    this.TopRightStreetPlacementNumber = 14;
                    this.RightStreetPlacementNumber = 13;
                    this.BottomRightStreetPlacementNumber = 48;
                    this.BottomLeftStreetPlacementNumber = 50;
                    this.LeftStreetPlacementNumber = 34;
                    break;
                case 10:
                    this.TopBuildingPlacementNumber = 29;
                    this.TopRightBuildingPlacementNumber = 28;
                    this.BottomRightBuildingPlacementNumber = 6;
                    this.BottomBuildingPlacementNumber = 7;
                    this.BottomLeftBuildingPlacementNumber = 8;
                    this.TopLeftBuildingPlacementNumber = 30;

                    this.TopLeftStreetPlacementNumber = 12;
                    this.TopRightStreetPlacementNumber = 11;
                    this.RightStreetPlacementNumber = 10;
                    this.BottomRightStreetPlacementNumber = 45;
                    this.BottomLeftStreetPlacementNumber = 47;
                    this.LeftStreetPlacementNumber = 13;
                    break;
                case 11:
                    this.TopBuildingPlacementNumber = 27;
                    this.TopRightBuildingPlacementNumber = 26;
                    this.BottomRightBuildingPlacementNumber = 25;
                    this.BottomBuildingPlacementNumber = 5;
                    this.BottomLeftBuildingPlacementNumber = 6;
                    this.TopLeftBuildingPlacementNumber = 28;

                    this.TopLeftStreetPlacementNumber = 9;
                    this.TopRightStreetPlacementNumber = 8;
                    this.RightStreetPlacementNumber = 7;
                    this.BottomRightStreetPlacementNumber = 6;
                    this.BottomLeftStreetPlacementNumber = 44;
                    this.LeftStreetPlacementNumber = 10;
                    break;
                case 12:
                    this.TopBuildingPlacementNumber = 10;
                    this.TopRightBuildingPlacementNumber = 9;
                    this.BottomRightBuildingPlacementNumber = 33;
                    this.BottomBuildingPlacementNumber = 3;
                    this.BottomLeftBuildingPlacementNumber = 34;
                    this.TopLeftBuildingPlacementNumber = 24;

                    this.TopLeftStreetPlacementNumber = 4;
                    this.TopRightStreetPlacementNumber = 2;
                    this.RightStreetPlacementNumber = 3;
                    this.BottomRightStreetPlacementNumber = 36;
                    this.BottomLeftStreetPlacementNumber = 38;
                    this.LeftStreetPlacementNumber = 5;
                    break;
                case 13:
                    this.TopBuildingPlacementNumber = 23;
                    this.TopRightBuildingPlacementNumber = 22;
                    this.BottomRightBuildingPlacementNumber = 31;
                    this.BottomBuildingPlacementNumber = 32;
                    this.BottomLeftBuildingPlacementNumber = 33;
                    this.TopLeftBuildingPlacementNumber = 9;

                    this.TopLeftStreetPlacementNumber = 1;
                    this.TopRightStreetPlacementNumber = 33;
                    this.RightStreetPlacementNumber = 32;
                    this.BottomRightStreetPlacementNumber = 15;
                    this.BottomLeftStreetPlacementNumber = 35;
                    this.LeftStreetPlacementNumber = 3;
                    break;
                case 14:
                    this.TopBuildingPlacementNumber = 21;
                    this.TopRightBuildingPlacementNumber = 20;
                    this.BottomRightBuildingPlacementNumber = 29;
                    this.BottomBuildingPlacementNumber = 30;
                    this.BottomLeftBuildingPlacementNumber = 31;
                    this.TopLeftBuildingPlacementNumber = 22;

                    this.TopLeftStreetPlacementNumber = 31;
                    this.TopRightStreetPlacementNumber = 16;
                    this.RightStreetPlacementNumber = 27;
                    this.BottomRightStreetPlacementNumber = 12;
                    this.BottomLeftStreetPlacementNumber = 14;
                    this.LeftStreetPlacementNumber = 32;
                    break;
                case 15:
                    this.TopBuildingPlacementNumber = 19;
                    this.TopRightBuildingPlacementNumber = 18;
                    this.BottomRightBuildingPlacementNumber = 27;
                    this.BottomBuildingPlacementNumber = 28;
                    this.BottomLeftBuildingPlacementNumber = 29;
                    this.TopLeftBuildingPlacementNumber = 20;

                    this.TopLeftStreetPlacementNumber = 28;
                    this.TopRightStreetPlacementNumber = 29;
                    this.RightStreetPlacementNumber = 30;
                    this.BottomRightStreetPlacementNumber = 9;
                    this.BottomLeftStreetPlacementNumber = 11;
                    this.LeftStreetPlacementNumber = 27;
                    break;
                case 16:
                    this.TopBuildingPlacementNumber = 16;
                    this.TopRightBuildingPlacementNumber = 15;
                    this.BottomRightBuildingPlacementNumber = 23;
                    this.BottomBuildingPlacementNumber = 9;
                    this.BottomLeftBuildingPlacementNumber = 10;
                    this.TopLeftBuildingPlacementNumber = 17;

                    this.TopLeftStreetPlacementNumber = 18;
                    this.TopRightStreetPlacementNumber = 19;
                    this.RightStreetPlacementNumber = 20;
                    this.BottomRightStreetPlacementNumber = 1;
                    this.BottomLeftStreetPlacementNumber = 2;
                    this.LeftStreetPlacementNumber = 17;
                    break;
                case 17:
                    this.TopBuildingPlacementNumber = 14;
                    this.TopRightBuildingPlacementNumber = 13;
                    this.BottomRightBuildingPlacementNumber = 21;
                    this.BottomBuildingPlacementNumber = 22;
                    this.BottomLeftBuildingPlacementNumber = 23;
                    this.TopLeftBuildingPlacementNumber = 15;

                    this.TopLeftStreetPlacementNumber = 21;
                    this.TopRightStreetPlacementNumber = 22;
                    this.RightStreetPlacementNumber = 23;
                    this.BottomRightStreetPlacementNumber = 31;
                    this.BottomLeftStreetPlacementNumber = 33;
                    this.LeftStreetPlacementNumber = 20;
                    break;
                case 18:
                    this.TopBuildingPlacementNumber = 12;
                    this.TopRightBuildingPlacementNumber = 11;
                    this.BottomRightBuildingPlacementNumber = 19;
                    this.BottomBuildingPlacementNumber = 20;
                    this.BottomLeftBuildingPlacementNumber = 21;
                    this.TopLeftBuildingPlacementNumber = 13;

                    this.TopLeftStreetPlacementNumber = 24;
                    this.TopRightStreetPlacementNumber = 25;
                    this.RightStreetPlacementNumber = 26;
                    this.BottomRightStreetPlacementNumber = 28;
                    this.BottomLeftStreetPlacementNumber = 16;
                    this.LeftStreetPlacementNumber = 23;
                    break;
            }

            this.TopBuildingGameObject = Game.GetGameObjectByName($"BuildingPlacement{this.TopBuildingPlacementNumber}");
            this.TopRightBuildingGameObject = Game.GetGameObjectByName($"BuildingPlacement{this.TopRightBuildingPlacementNumber}");
            this.BottomRightBuildingGameObject = Game.GetGameObjectByName($"BuildingPlacement{this.BottomRightBuildingPlacementNumber}");
            this.BottomBuildingGameObject = Game.GetGameObjectByName($"BuildingPlacement{this.BottomBuildingPlacementNumber}");
            this.BottomLeftBuildingGameObject = Game.GetGameObjectByName($"BuildingPlacement{this.BottomLeftBuildingPlacementNumber}");
            this.TopLeftBuildingGameObject = Game.GetGameObjectByName($"BuildingPlacement{this.TopLeftBuildingPlacementNumber}");

            this.TopLeftStreetGameObject = Game.GetGameObjectByName($"StreetPlacement{this.TopLeftStreetPlacementNumber}");
            this.TopRightStreetGameObject = Game.GetGameObjectByName($"StreetPlacement{this.TopRightStreetPlacementNumber}");
            this.RightStreetGameObject = Game.GetGameObjectByName($"StreetPlacement{this.RightStreetPlacementNumber}");
            this.BottomRightStreetGameObject = Game.GetGameObjectByName($"StreetPlacement{this.BottomRightStreetPlacementNumber}");
            this.BottomLeftStreetGameObject = Game.GetGameObjectByName($"StreetPlacement{this.BottomLeftStreetPlacementNumber}");
            this.LeftStreetGameObject = Game.GetGameObjectByName($"StreetPlacement{this.LeftStreetPlacementNumber}");
        }

        /// <summary>
        /// Creates a new <see cref="Field"/> object from a <see cref="UnityEngine.GameObject"/> (Tile in hierarchy)
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static Field CreateNewField(GameObject gameObject, int number)
        {
            Field newField = new()
            {
                GameObject = gameObject,
                FieldNumber = number,
                DiceNumber = 0,
                IsRobberPlacedOnThisField = false
            };

            newField.InitializeBuildingsAndStreets();

            return newField;
        }

        #endregion Methods

    }
}
