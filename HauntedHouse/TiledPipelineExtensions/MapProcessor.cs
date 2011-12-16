﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;
using System;

namespace HauntedHouseContentPipeline
{
    // Each tile has a texture, source rect, and sprite effects.
    [ContentSerializerRuntimeType("HauntedHouse.Tile, HauntedHouse")]
    public class MapTileContent
    {
        public ExternalReference<Texture2DContent> Texture;
        public Rectangle SourceRectangle;
        public SpriteEffects SpriteEffects;
        public bool IsShadowCaster;
    }

    // For each layer, we store the size of the layer and the tiles.
    [ContentSerializerRuntimeType("HauntedHouse.Layer, HauntedHouse")]
    public class MapLayerContent
    {
        public int Width;
        public int Height;
        public MapTileContent[] Tiles;
    }

    // For the map itself, we just store the size, tile size, and a list of layers.
    [ContentSerializerRuntimeType("HauntedHouse.Map, HauntedHouse")]
    public class HauntedHouseMapContent
    {
        public int TileWidth;
        public int TileHeight;
        public List<MapLayerContent> Layers = new List<MapLayerContent>();
    }

    [ContentProcessor(DisplayName = "TMX Processor - HauntedHouse")]
    public class MapProcessor : ContentProcessor<MapContent, HauntedHouseMapContent>
    {
        public override HauntedHouseMapContent Process(MapContent input, ContentProcessorContext context)
        {
            // build the textures
            TiledHelpers.BuildTileSetTextures(input, context);

            // generate source rectangles
            TiledHelpers.GenerateTileSourceRectangles(input);

            // now build our output, first by just copying over some data
            HauntedHouseMapContent output = new HauntedHouseMapContent
            {
                TileWidth = input.TileWidth,
                TileHeight = input.TileHeight
            };

            // iterate all the layers of the input
            foreach (LayerContent layer in input.Layers)
            {
                // we only care about tile layers in our demo
                TileLayerContent tileLayerContent = layer as TileLayerContent;
                if (tileLayerContent != null)
                {
                    // create the new layer
                    MapLayerContent outLayer = new MapLayerContent
                    {
                        Width = tileLayerContent.Width,
                        Height = tileLayerContent.Height,
                    };

                    // we need to build up our tile list now
                    outLayer.Tiles = new MapTileContent[tileLayerContent.Data.Length];
                    for (int i = 0; i < tileLayerContent.Data.Length; i++)
                    {
                        // get the ID of the tile
                        uint tileID = tileLayerContent.Data[i];

                        // use that to get the actual index as well as the SpriteEffects
                        int tileIndex;
                        SpriteEffects spriteEffects;
                        TiledHelpers.DecodeTileID(tileID, out tileIndex, out spriteEffects);

                        // figure out which tile set has this tile index in it and grab
                        // the texture reference and source rectangle.
                        ExternalReference<Texture2DContent> textureContent = null;
                        Rectangle sourceRect = new Rectangle();
                        bool isShadowCaster = new bool();

                        // iterate all the tile sets
                        foreach (var tileSet in input.TileSets)
                        {
                            // if our tile index is in this set
                            if (tileIndex - tileSet.FirstId < tileSet.Tiles.Count)
                            {
                                // store the texture content and source rectangle
                                textureContent = tileSet.Texture;
                                sourceRect = tileSet.Tiles[(int)(tileIndex - tileSet.FirstId)].Source;
                                //isShadowCaster = Convert.ToBoolean(tileSet.Tiles[(int)(tileIndex - tileSet.FirstId)].Properties["IsShadowCaster"]);

                                // and break out of the foreach loop
                                break;
                            }
                        }

                        // now insert the tile into our output
                        outLayer.Tiles[i] = new MapTileContent
                        {
                            Texture = textureContent,
                            SourceRectangle = sourceRect,
                            SpriteEffects = spriteEffects,
                            IsShadowCaster = isShadowCaster
                        };
                    }

                    // add the layer to our output
                    output.Layers.Add(outLayer);
                }
            }

            // return the output object. because we have ContentSerializerRuntimeType attributes on our
            // objects, we don't need a ContentTypeWriter and can just use the automatic serialization.
            return output;
        }
    }
}
