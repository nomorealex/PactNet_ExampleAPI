namespace Provider
{
    /// <summary>
    /// Model representing a Order:
    ///     Id: Unique Integer automatically generated
    ///     Name: Name of the order
    ///     Description: A brief description of the order.
    /// </summary>
    public class Orders
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Orders(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
        public override string ToString()
        {
            return $"Order -> [Id: {this.Id}, Name: {this.Name}, Description: {this.Description}]";
        }
    }
}
