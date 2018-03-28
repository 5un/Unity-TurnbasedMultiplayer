using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTXBaseUnit: Unit {

	public Color LeadingColor;

	public override void Initialize()
	{
		base.Initialize ();
		transform.position += new Vector3 (0, 1, 0);
		GetComponent<Renderer> ().material.color = LeadingColor;
	}

	public override void MarkAsFriendly()
	{
		GetComponent<Renderer> ().material.color = LeadingColor + new Color (0.8f, 1, 0.8f);
	}

	public override void MarkAsReachableEnemy()
	{
		GetComponent<Renderer> ().material.color = LeadingColor + Color.red;
	}

	public override void MarkAsSelected()
	{
		GetComponent<Renderer>().material.color = LeadingColor + Color.green;
	}

	public override void MarkAsDestroyed()
	{
		GetComponent<Renderer> ().material.color = Color.black;
	}

	public override void MarkAsAttacking(Unit other)
	{
		GetComponent<Renderer> ().material.color = Color.white;
	}

	public override void MarkAsDefending(Unit other)
	{
		GetComponent<Renderer> ().material.color = Color.black;
	}

	public override void MarkAsFinished()
	{
		GetComponent<Renderer> ().material.color = LeadingColor;
	}

	public override void UnMark()
	{
		GetComponent<Renderer> ().material.color = LeadingColor;
	}

}
