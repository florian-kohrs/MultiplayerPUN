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

    public Vector2Int dimensions;

    public MapOccupation[,] occupationMap;

    [SerializeField]
    protected List<MapOccupationObject> allMapOccupationObjects;

    public List<MapOccupationObject> randomReceivableMapOccupationObjects;

    protected Vector2Int startPoint;

    protected readonly Vector3 PlayerOffset = new Vector3(0, 0.666f, 0);

    public void SetStartPoint(Vector2Int start, int startAreaIndex)
    {
        startPoint = start;
        if (!PlaceGeneration(startAreaIndex, start.x, start.y, 0))
            throw new Exception("Start Area must be able to spawn!");

    }

    public void PositionPlayers(List<Player> players)
    {
        foreach (Player p in players)
        {
            Vector3 pos = MapIndexToGlobalPosition(startPoint) + PlayerOffset;
            p.GetActualPlayer().position = pos;
            //p.body.MovePosition(pos);
        }
    }

    private void Awake()
    {
        occupationMap = new MapOccupation[dimensions.x,dimensions.y];
    }

    private void Start()
    {
        view = PhotonView.Get(this);
    }

    protected List<MapOccupationObject> ListFromIndex(int index)
    {
        if (index == 0)
            return allMapOccupationObjects;
        else
            return randomReceivableMapOccupationObjects;
    }

    protected enum OccupationList { All = 0, OnlyRandomRotation = 1}

    public bool PlaceGeneration(int occupationIndex, int originX, int originY, int rotationIndex)
    {
        //Broadcast.SafeRPC(view, nameof(Place))
        return Place(occupationIndex, originX, originY, rotationIndex, OccupationList.All, false);
    }

    
    public bool PlaceDuringRounds(int occupationIndex, int originX, int originY, int rotationIndex)
    {
        bool canPlace = CanPlace(occupationIndex, originX, originY, rotationIndex, OccupationList.OnlyRandomRotation);
        if (canPlace)
        {
            Broadcast.SafeRPC(View, nameof(Place), RpcTarget.All,
                () => { Place(occupationIndex, originX, originY, rotationIndex, OccupationList.OnlyRandomRotation, true); },
                occupationIndex, originX, originY, rotationIndex, OccupationList.OnlyRandomRotation, true);
        }
        return canPlace;
    }

    protected bool CanPlace(int occupationIndex, int originX, int originY, int rotationIndex, OccupationList listIndex)
    {
        MapOccupationObject occupation = ListFromIndex((int)listIndex)[occupationIndex];
        Vector2Int origin = new Vector2Int(originX, originY);
        return AreAllInBounds(origin, rotationIndex, occupation) && IsSpaceNotOccupied(occupation, origin, rotationIndex);
    }

    [PunRPC]
    protected bool Place(int occupationIndex, int originX, int originY, int rotationIndex, OccupationList listIndex, bool destroyable)
    {
        MapOccupationObject occupation = ListFromIndex((int)listIndex)[occupationIndex];
        Vector2Int origin = new Vector2Int(originX, originY);
        bool result = AreAllInBounds(origin, rotationIndex, occupation) && (occupation.isBomb || IsSpaceNotOccupied(occupation,origin,rotationIndex));
        if(result)
        {
            MapOccupation mapOccupation = new MapOccupation(occupation,origin,rotationIndex, destroyable);
            if (!occupation.isBomb)
            {
                OccupySpace(mapOccupation);
            }
            Quaternion rotation = Quaternion.identity;
            Vector3 scale = Vector3.one;

            if(occupation.mirrorInsteadOfRotate)
                scale = new Vector3(Mathf.Pow(-1, rotationIndex), 1, 1);
            else
                rotation = GetObjectRotation(rotationIndex);

            mapOccupation.gameObject = Spawn(mapOccupation, MapIndexToGlobalPosition(origin), rotation, scale);
        }
        return result;
    }


    protected GameObject Spawn(MapOccupation occupation, Vector3 pos, Quaternion rotation, Vector3 scale)
    {
        GameObject g = Instantiate(occupation.occupationObject.prefab, pos, rotation);
        g.transform.localScale = scale;
        BasePlaceableBehaviours placeableBehaviour = g.GetComponent<BasePlaceableBehaviours>();
        if (placeableBehaviour != null)
        {
            placeableBehaviour.occupation = occupation;
            placeableBehaviour.OnPlace(this);
        }
        return g;
    }

    public void DestroyOccupations(MapOccupation mapObject)
    {
        foreach (var spot in mapObject.GetAllOccupations())
        {
            if (IsInBounds(spot))
            {
                MapOccupation occ = occupationMap[spot.x, spot.y];
                if (occ == null)
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
                if (occupationMap[spot.x, spot.y] == null)
                    Debug.LogError("Occupation map should have value here!");

                occupationMap[spot.x, spot.y] = null;
            }
        }
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
        bool result = true;
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
        return pos.x >= 0 && pos.y >= 0 && pos.x < dimensions.x && pos.y < dimensions.y;
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
            if(occupationMap[spot.x, spot.y] != null)
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
            occupationMap[spot.x, spot.y] = occupation;
        }
    }

}

