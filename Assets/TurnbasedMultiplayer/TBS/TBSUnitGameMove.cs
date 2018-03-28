using UnityEngine;
using System.Collections;

[System.Serializable]
public class TBSUnitGameMove: TBSBaseGameMove
{
	public string fromCell;
	public string toCell;
	public TBSUnitGameMove() {
		base.type = "unitMove";
	}

}

