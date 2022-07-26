using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDesigner : MonoBehaviourPun
{

    public List<EquipableItemAsset> hats;
    public List<EquipableItemAsset> hair;
    public List<EquipableItemAsset> beards;
    public List<EquipableItemAsset> other;

    public GameObject selectionPrefab;
    public Transform equipParent;
    public RectTransform parent;
    public float distanceBetween;

    public Vector2 startPos;

    protected List<ListSelection> selections;

    protected void Start()
    {
        bool show = !PhotonNetwork.IsConnected || photonView.IsMine;
        StartDesigner(show);
        if (show)
        {
            GameCycle.AddGameStartCallback(HideDesigners);
        }
    }

    [PunRPC]
    protected void UpdatePlayerLooks(int hatIndex, int hairIndex, int beardIndex, int otherIndex)
    {
        selections[0].UpdateSelection(hatIndex);
        selections[1].UpdateSelection(hairIndex);
        selections[2].UpdateSelection(beardIndex);
        selections[3].UpdateSelection(otherIndex);
    }

    public void HideDesigners()
    {
        foreach (ListSelection l in selections)
        {
            l.Hide();
        }
    }

    protected object GetRpcReadyIndices()
    {
        return selections.Select(s => (object)s.CurrentSelectedIndex).ToArray();
    }

    protected void DesignChanged()
    {
        Broadcast.SafeRPC(photonView, nameof(UpdatePlayerLooks), RpcTarget.Others, null, 
            selections[0].CurrentSelectedIndex,
            selections[1].CurrentSelectedIndex,
            selections[2].CurrentSelectedIndex,
            selections[3].CurrentSelectedIndex);
    }

    protected IEnumerable<KeyValuePair<string, List<EquipableItemAsset>>> GetItemLists()
    {
        yield return KeyValuePair.Create("Hats",hats);
        yield return KeyValuePair.Create("Hair", hair);
        yield return KeyValuePair.Create("Beards", beards);
        yield return KeyValuePair.Create("Other", other);
    }



    public void StartDesigner(bool display)
    {
        selections = new List<ListSelection>();
        int index = 0;
        foreach(KeyValuePair<string,List<EquipableItemAsset>> items in GetItemLists())
        {
            GameObject g = Instantiate(selectionPrefab, parent);
            ListSelection selection = g.GetComponent<ListSelection>();
            selection.Initialize(startPos + new Vector2(0,distanceBetween * index), items.Key, items.Value, equipParent, DesignChanged);
            if (display)
            {
                selection.OpenSelection();
            }
            selections.Add(selection);
            index++;
        }
    }

}
