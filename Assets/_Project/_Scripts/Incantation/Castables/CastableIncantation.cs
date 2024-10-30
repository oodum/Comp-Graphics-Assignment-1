using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventBus;
using IncantationSystem;
using MusicEngine;
using Scoring;
using UnityEngine;
public class CastableIncantation {
	readonly Incantation Incantation;
	List<ScoreType> ratings;
	public List<Note> Notes => Incantation.Notes;
	public bool IsComplete => ratings.All(rating => rating != ScoreType.None);
	public bool IsFailed => Confidence <= 0;
	public int Count => Notes.Count;
	public int CurrentIndex => ratings.Count(rating => rating != ScoreType.None);
	public string Name => Incantation.Name;
	public float Confidence = 1;

	readonly EventBinding<BeatEvent> beatEvent;

	public CastableIncantation(Incantation incantation) {
		Incantation = incantation;
		CreateRatingsList(); // this is a code smell (logic in constructor)
		Reset();
	}

	void CreateRatingsList() {
		ratings = new(new ScoreType[Notes.Count]);
	}

	void SetRatingsToNone() {
		for (var i = 0; i < ratings.Count; i++) ratings[i] = ScoreType.None;
	}

	public Note GetNextUnplayedNote() {
		return Notes.Find(note => ratings[Notes.IndexOf(note)] == ScoreType.None);
	}

	public bool TryGetFirstUnplayedNote(out Note note) {
		note = null;
		if (ratings.Last() != ScoreType.None) return false;
		note = GetNextUnplayedNote();
		return true;
	}

	public Note GetLastPlayedNote(out ScoreType rating) {
		var note =Notes.FindLast(note => ratings[Notes.IndexOf(note)] != ScoreType.None);
		var index = Notes.IndexOf(note);
		rating = (index == -1) ? ScoreType.None : ratings[index];
		return note;
	}

	public bool TryGetLastPlayedNote(out Note note, out ScoreType rating) {
		note = null;
		rating = ScoreType.None;
		if (ratings.First() == ScoreType.None) return false;
		note = GetLastPlayedNote(out rating);
		return true;
	}

	public ScoreType GetRating(Note note) {
		return ratings[Notes.IndexOf(note)];
	}

	public void Play(ScoreType rating) => Play(rating, 0);

	public void Play(ScoreType rating, float confidencePenalization) {
		if (!TryGetFirstUnplayedNote(out var note)) {
			Debug.Log("No unplayed notes left");
			return;
		}
		SubtractConfidence(confidencePenalization);
		ratings[Notes.IndexOf(note)] = rating;
	}
	void SubtractConfidence(float confidencePenalization) {
		// Clamp the penalization to [0,1] so that the subtraction doesn't increase the confidence
		confidencePenalization = Mathf.Clamp01(confidencePenalization);
		Confidence -= confidencePenalization;
		if (Confidence < 0) Confidence = 0;
	}

	public void Reset() {
		SetRatingsToNone();
		Confidence = 1;
	}
}
