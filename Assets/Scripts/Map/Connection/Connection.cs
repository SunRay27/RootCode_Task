using System;
using System.Collections.Generic;

public partial class MapController
{
    /// <summary>
    /// Represents bi-directional connection between two points
    /// </summary>
    private class Connection
    {
        public int weight;
        public MapPoint point1;
        public MapPoint point2;

        //yep it is inside model
        public ConnectionView view;

        public Connection(MapPoint point1, MapPoint point2, int weight)
        {
            this.weight = weight;
            this.point1 = point1;
            this.point2 = point2;
        }

        public void InitView(ConnectionView prefab)
        {
            if (view != null)
                Destroy(view.gameObject);

            view = Instantiate(prefab);
            view.Init(point1.transform.position, point2.transform.position, weight);
        }

        public override bool Equals(object obj)
        {
            //skip weight equalitiy comparison

            return obj is Connection connection &&

                ((EqualityComparer<MapPoint>.Default.Equals(point1, connection.point1) &&
                   EqualityComparer<MapPoint>.Default.Equals(point2, connection.point2))

                   ||

                   (EqualityComparer<MapPoint>.Default.Equals(point1, connection.point2) &&
                   EqualityComparer<MapPoint>.Default.Equals(point2, connection.point1)));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(point1, point2) + HashCode.Combine(point2, point1);
        }
    }
}
