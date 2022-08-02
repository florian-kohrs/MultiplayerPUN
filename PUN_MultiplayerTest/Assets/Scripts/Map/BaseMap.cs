using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BaseMap : MonoBehaviourPun
{

    public const float MAP_FIELD_SPACING = 1;
    public const float HALF_SPACING = MAP_FIELD_SPACING / 2;

    protected PhotonView view;

    protected PhotonView View
    {
        get 
        {
            if (view == null)
                view = PhotonView.Get(this);
            return view;
        }
    }

    public Vector3 Anchor => transform.position;

    public Vector2 Anchor2D => new Vector2(transform.position.x, transform.position.y);

    protected Vector2Int dimensions;

    protected virtual bool BombsDestroyEverything => false;

    public Vector2Int Dimensions
    {
        get { return dimensions; }
        protected set
        {
            dimensions = value;
            occupationMap = new MapOccupation[dimensions.x, dimensions.y];
        }
    }

    //public virtual Vector2Int dimensions => dimensions;

    public MapOccupation[,] occupationMap;
    public MapOccupation[,] OccupationMap
    {
        get
        {
            if (occupationMap == null)
                occupationMap = new MapOccupation[Dimensions.x, Dimensions.y];
            return occupationMap;
        }
    }

    public MapOccupationList mapOccupations;

    public List<MapOccupationObject> AllMapOccupationObjects => mapOccupations.MapOccupations;


    public MapOccupationList randomReceivableMapOccupationObjects;

    protected Vector2Int startPoint;

    public void SetStartPoint(Vector2Int startPoint)
    {
        this.startPoint = startPoint;
    }


    protected readonly Vector3 PlayerOffset = new Vector3(0, 0.666f, 0);

    public void ResetMap()
    {
        int xLength = OccupationMap.GetLength(0);
        int yLength = OccupationMap.GetLength(1);
        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                MapOccupation occ = OccupationMap[x, y];
                if (occ == null)
                    continue;

                DestroyOccupations(occ);
            }
        }
        occupationMap = new MapOccupation[Dimensions.x, Dimensions.y];
    }

    public void LoadMapDesign(MapDesign mapDesign)
    {
        ResetMap();
        Dimensions = mapDesign.dimensions;
        foreach (var item in mapDesign.GetMapDesign())
        {
            if (item.Element == null || item.Element.occupationObject == null)
                continue;
            Place(-1,item.Element.occupationObject, item.Index, item.Element.orientation, false);
        }
    }

    public void SetStartPoint(Vector2Int start, int startAreaIndex)
    {
        startPoint = start;
        if (!PlaceGeneration(startAreaIndex, start.x, start.y, 0))
            throw new Exception("Start Area must be able to spawn!");

    }

    public void PositionPlayers()
    {
        GameCycle.IterateOverPlayers((p) =>
        {
            Vector3 pos = MapIndexToGlobalPosition(startPoint) + PlayerOffset;
            p.GetActualPlayer().position = pos;
        });
    }


    private void Start()
    {
        view = PhotonView.Get(this);
    }

    protected List<MapOccupationObject> ListFromIndex(int index)
    {
        if (index == 0)
            return AllMapOccupationObjects;
        else
            return randomReceivableMapOccupationObjects.MapOccupations;
    }

    protected enum OccupationList { All = 0, OnlyRandomRotation = 1}

    public bool PlaceGeneration(int occupationIndex, int originX, int originY, int rotationIndex)
    {
        //Broadcast.SafeRPC(view, nameof(Place))
        return Place(-1, occupationIndex, originX, originY, rotationIndex, OccupationList.All, false);
    }


    public bool PlaceDuringRounds(int occupationIndex, int originX, int originY, int rotationIndex)
    {
        bool canPlace = CanPlace(occupationIndex, originX, originY, rotationIndex, OccupationList.OnlyRandomRotation);
        if (canPlace)
        {
            int playerId = PhotonNetwork.IsConnected ? PhotonNetwork.LocalPlayer.ActorNumber : 0;
            Broadcast.SafeRPC(View, nameof(Place), RpcTarget.All,
                () => { Place(playerId, occupationIndex, originX, originY, rotationIndex, OccupationList.OnlyRandomRotation, true); },
                playerId, occupationIndex, originX, originY, rotationIndex, OccupationList.OnlyRandomRotation, true);
        }
        return canPlace;
    }

    protected bool CanPlace(int occupationIndex, int originX, int originY, int rotationIndex, OccupationList listIndex)
    {
        MapOccupationObject occupation = ListFromIndex((int)listIndex)[occupationIndex];
        Vector2Int origin = new Vector2Int(originX, originY);
        return CanPlace(occupation, origin, rotationIndex);
    }

    protected bool CanPlace(MapOccupationObject occupation, Vector2Int origin, int rotationIndex)
    {
        return AreAllInBounds(origin, rotationIndex, occupation) && (occupation.isBomb || IsSpaceNotOccupied(occupation, origin, rotationIndex));
    }



    [PunRPC]
    protected bool Place(int playerId, int occupationIndex, int originX, int originY, int rotationIndex, OccupationList listIndex, bool destroyable)
    {
        MapOccupationObject occupation = ListFromIndex((int)listIndex)[occupationIndex];
        Vector2Int origin = new Vector2Int(originX, originY);
        return Place(playerId, occupation, origin, rotationIndex, destroyable);
    }

    protected bool Place(int playerId, MapOccupationObject occupation, Vector2Int origin, int rotationIndex, bool destroyable)
    {
        bool result = CanPlace(occupation, origin, rotationIndex);
        if (result)
        {
            MapOccupation mapOccupation = new MapOccupation(occupation, origin, rotationIndex, destroyable);
            if (!occupation.isBomb)
            {
                OccupySpace(mapOccupation);
            }
            Quaternion rotation = Quaternion.identity;
            Vector3 scale = Vector3.one;

            if (occupation.mirrorInsteadOfRotate)
                scale = new Vector3(Mathf.Pow(-1, rotationIndex), 1, 1);
            else
                rotation = GetObjectRotation(rotationIndex);

            mapOccupation.gameObject = Spawn(playerId, mapOccupation, MapIndexToGlobalPosition(origin), rotation, scale);
        }
        return result;
    }



    protected GameObject Spawn(int playerId,MapOccupation occupation, Vector3 pos, Quaternion rotation, Vector3 scale)
    {
        GameObject g = Instantiate(occupation.occupationObject.prefab, pos, rotation);
        g.transform.localScale = scale;
        BasePlaceableBehaviours placeableBehaviour = g.GetComponent<BasePlaceableBehaviours>();
        if (placeableBehaviour != null)
        {
            placeableBehaviour.occupation = occupation;
            placeableBehaviour.OnPlace(this);
        }
        IHasPlacedById placedBy = g.GetComponentInChildren<IHasPlacedById>();
        if(placedBy != null)
        {
            placedBy.PlacedByPlayerID = playerId;
        }    
        occupation.occupationObject.ApplyToObject(g);
        return g;
    }

    public void DestroyOccupations(MapOccupation mapObject)
    {
        foreach (var spot in mapObject.GetAllOccupations())
        {
            if (IsInBounds(spot))
            {
                MapOccupation occ = OccupationMap[spot.x, spot.y];
                if (occ == null || (!occ.destroyable && !BombsDestroyEverything))
                    continue;

                ClearOccupation(occ);
            }
        }
    }

    protected void ClearOccupation(MapOccupation occupation)
    {
        Destroy(occupation.gameObject);
        foreach (var spot in occupation.GetAllOccupations())
        {
            if (IsInBounds(spot))
            {
                if (OccupationMap[spot.x, spot.y] == null)
                    Debug.LogError("Occupation map should have value here!");

                OccupationMap[spot.x, spot.y] = null;
            }
        }
        OnDestroyedOccupation(occupation.origin);
    }

    protected virtual void OnDestroyedOccupation(Vector2Int origin)
    {

    }

    public Vector3 MapIndexToGlobalPosition(Vector2Int index)
    {
        return transform.position + (index.ToVector2() * MAP_FIELD_SPACING).LiftVectorOnXY();
    }


    public Quaternion GetObjectRotation(int rotation)
    {
        return Quaternion.Euler(0, 0, -rotation * 90);
    }

    protected bool AreAllInBounds(Vector2Int pos, int rotation, MapOccupationObject mapObject)
    {
        bool result = IsInBounds(pos);
        if (!result)
            return result;
        foreach (var spot in mapObject.GetAllOccupations(pos, rotation))
        {
            if (!IsInBounds(spot))
            {
                result = false;
                break;
            }
        }
        return result;
    }

    protected bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < Dimensions.x && pos.y < Dimensions.y;
    }


    public Vector2Int PositionToIndex(Vector2 pos)
    {
        Vector2 localPos = pos - Anchor2D;
        Vector2Int index = new Vector2Int(
           Mathf.FloorToInt(localPos.x / MAP_FIELD_SPACING),
           Mathf.FloorToInt(localPos.y / MAP_FIELD_SPACING));
        return index;
    }

    protected bool IsSpaceNotOccupied(MapOccupationObject occupation, Vector2Int origin, int rotation)
    {
        bool result = true;
        foreach(var spot in occupation.GetAllOccupations(origin,rotation))
        {
            if(OccupationMap[spot.x, spot.y] != null)
            {
                result = false;
                break;
            }
        }
        return result;
    }

    protected void OccupySpace(MapOccupation occupation)
    {
        foreach (var spot in occupation.GetAllOccupations())
        {
            OccupationMap[spot.x, spot.y] = occupation;
        }
    }

}

