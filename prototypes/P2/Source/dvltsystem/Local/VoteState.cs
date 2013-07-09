namespace DVLTerminal.Local
{
	/// <summary>
	/// Defines the different states that a vote can be in
	/// </summary>
	public enum VoteState
	{
		/// <summary>
		/// The vote has been created but is not yet checked
		/// </summary>
		Initializing, 
		
		/// <summary>
		/// A message has been broadcasted that this person has voted
		/// </summary>
		MessageSent, 
		
		/// <summary>
		/// The vote has been completely processed
		/// </summary>
		Done
	}
}