﻿using System;
using System.Collections.Generic;
#if StkComponents
using AGI.Foundation.Cesium.Advanced;
using AGI.Foundation.Coordinates;
using AGI.Foundation.Infrastructure;
using AGI.Foundation.Time;
#else
using CesiumLanguageWriter.Advanced;
#endif

#if StkComponents
namespace AGI.Foundation.Cesium
#else
namespace CesiumLanguageWriter
#endif
{
    /// <summary>
    /// A <see cref="CesiumPropertyWriter{T}"/> used to write the cartesian positions property that
    /// optionally has different values over different intervals of time.  Instances of this class generally should not
    /// be constructed directly, but should instead be obtained from a <see cref="CesiumPropertyWriter{T}"/>.
    /// </summary>
    public class PositionListCesiumWriter : CesiumValuePropertyWriter<IEnumerable<Cartesian>, PositionListCesiumWriter>
    {
        private readonly Lazy<ICesiumValuePropertyWriter<IEnumerable<Cartographic>>> m_cartographic;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public PositionListCesiumWriter(string propertyName)
            : base(propertyName)
        {
            m_cartographic = new Lazy<ICesiumValuePropertyWriter<IEnumerable<Cartographic>>>(CreateCartographicAdaptor, false);
        }

        /// <inheritdoc />
        private PositionListCesiumWriter(PositionListCesiumWriter existingInstance)
            : base(existingInstance)
        {
            m_cartographic = new Lazy<ICesiumValuePropertyWriter<IEnumerable<Cartographic>>>(CreateCartographicAdaptor, false);
        }

        /// <inheritdoc />
        public override PositionListCesiumWriter Clone()
        {
            return new PositionListCesiumWriter(this);
        }

        /// <summary>
        /// Returns a wrapper for this instance that implements <see cref="ICesiumValuePropertyWriter{T}"/> to
        /// write a list of <see cref="Cartographic"/> values.  Because the returned instance is a wrapper
        /// for this instance, you may call <see cref="ICesiumElementWriter.Close"/> on either this instance
        /// or the wrapper, but you must not call it on both.
        /// </summary>
        /// <returns>The wrapper.</returns>
        public ICesiumValuePropertyWriter<IEnumerable<Cartographic>> AsCartographic()
        {
            return m_cartographic.Value;
        }

        /// <summary>
        /// Writes the value of the property for this interval as an array of <see cref="Cartesian"/>
        /// </summary>
        /// <param name="positions">The positions.</param>
        public override void WriteValue(IEnumerable<Cartesian> positions)
        {
            OpenIntervalIfNecessary();

            Output.WritePropertyName("cartesian");
            Output.WriteStartSequence();
            foreach (var position in positions)
            {
                Output.WriteValue(position.X);
                Output.WriteValue(position.Y);
                Output.WriteValue(position.Z);
            }
            Output.WriteEndSequence();
        }

        /// <summary>
        /// Writes the value of the property for this interval as an array of <see cref="Cartographic"/>
        /// </summary>
        /// <param name="positions">The positions.</param>
        public void WriteValue(IEnumerable<Cartographic> positions)
        {
            OpenIntervalIfNecessary();

            Output.WritePropertyName("cartographicRadians");
            Output.WriteStartSequence();
            foreach (var position in positions)
            {
                Output.WriteValue(position.Longitude);
                Output.WriteValue(position.Latitude);
                Output.WriteValue(position.Height);
            }
            Output.WriteEndSequence();
        }

        private ICesiumValuePropertyWriter<IEnumerable<Cartographic>> CreateCartographicAdaptor()
        {
            return new CesiumWriterAdaptor<PositionListCesiumWriter, IEnumerable<Cartographic>>(
                this, (me, value) => me.WriteValue(value));
        }
    }
}