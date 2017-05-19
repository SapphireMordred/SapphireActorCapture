using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireActorCapture.Models
{
    public class Map
    {
        /// <summary>
        /// Index: 4
        /// </summary>
        public UInt32 mapMarkerRange;
        /// <summary>
        /// Index: 5
        /// </summary>
        public String identifier;
        /// <summary>
        /// Index: 6
        /// </summary>
        public UInt32 sizeFactor;
        /// <summary>
        /// Index: 9
        /// </summary>
        public UInt32 placeNameRegion;
        /// <summary>
        /// Index: 11
        /// </summary>
        public UInt32 placeNameSub;
        /// <summary>
        /// Index: 14
        /// </summary>
        public UInt32 territoryType;
        /// <summary>
        /// Index: 3
        /// </summary>
        public UInt32 hierarchy;
        /// <summary>
        /// Index: 10
        /// </summary>
        public UInt32 placeName;

        public Map(uint mapMarkerRange, string identifier, uint sizeFactor, uint placeNameRegion, uint placeNameSub, uint territoryType, uint hierarchy, uint placeName)
        {
            this.mapMarkerRange = mapMarkerRange;
            this.identifier = identifier;
            this.sizeFactor = sizeFactor;
            this.placeNameRegion = placeNameRegion;
            this.placeNameSub = placeNameSub;
            this.territoryType = territoryType;
            this.hierarchy = hierarchy;
            this.placeName = placeName;
        }
    }
}
