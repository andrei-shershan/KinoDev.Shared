namespace KinoDev.Shared.Enums
{
    /// <summary>
    /// IMPORTANT!
    /// This values are used in the database.
    /// Do not chang numbers or order of the values.
    /// Add new values at the end of the list.
    /// </summary>
    public enum OrderState
    {
        New = 0,
        Pending = 10,
        Processing = 20,
        Completed = 30,
        Cancelled = 40
    }
}
