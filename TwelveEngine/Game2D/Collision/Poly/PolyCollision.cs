using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine.Game2D.Collision.Poly {
    public sealed class PolyCollision:CollisionInterface {

        /* https://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland_algorithm */

        [Flags]
        private enum Partition {
            Inside = 0,
            Left = 1,
            Right = 2,
            Bottom = 4,
            Top = 8
        }

        public Line[] Lines { get; set; }

        private Line[] GetLocalGraph(Hitbox source) {
            //todo
            return Lines;
        }

        private Partition GetPartition(Hitbox hitbox,Vector2 point) {
            Partition partition = Partition.Inside;

            if(point.X < hitbox.X) {
                partition |= Partition.Left;
            } else if(point.X > hitbox.Right) {
                partition |= Partition.Right;
            }

            if(point.Y < hitbox.Y) {
                partition |= Partition.Top;
            } else if(point.Y > hitbox.Bottom) {
                partition |= Partition.Bottom;
            }

            return partition;
        }

		private enum IntersectionResult { Outside, Inside, Intersects }

        private (IntersectionResult Intersection,Line? ClipLine) LineIntersects(Hitbox hitbox,Line line) {

			Partition partitionA = GetPartition(hitbox,line.A), partitionB = GetPartition(hitbox,line.B);

			bool intersects = false;

			while(true) {
				if((partitionA | partitionB) == 0) {
					if(intersects) {
						return (IntersectionResult.Intersects, line);
                    } else {
						return (IntersectionResult.Inside, null);
					}
				} else if((partitionA & partitionB) != 0) {
					return (IntersectionResult.Outside, null);
				} else {
					float x = 0f, y = 0f;
					intersects = true;

					Partition partitionOut = partitionB > partitionA ? partitionB : partitionA;

					if((partitionOut & Partition.Bottom) != 0) {
						x = line.A.X + (line.B.X - line.A.X) * (hitbox.Bottom - line.A.Y) / (line.B.Y - line.A.Y);
						y = hitbox.Bottom;
					} else if((partitionOut & Partition.Top) != 0) {
						x = line.A.X + (line.B.X - line.A.X) * (hitbox.Y - line.A.Y) / (line.B.Y - line.A.Y);
						y = hitbox.Y;
					} else if((partitionOut & Partition.Right) != 0) {
						y = line.A.Y + (line.B.Y - line.A.Y) * (hitbox.Right - line.A.X) / (line.B.X - line.A.X);
						x = hitbox.Right;
					} else if((partitionOut & Partition.Left) != 0) {
						y = line.A.Y + (line.B.Y - line.A.Y) * (hitbox.X - line.A.X) / (line.B.X - line.A.X);
						x = hitbox.X;
					}

					if(partitionOut == partitionA) {
						line.A = new Vector2(x,y);
						partitionA = GetPartition(hitbox,line.A);
					} else {
						line.B = new Vector2(x,y);
						partitionB = GetPartition(hitbox,line.B);
					}
				}
			}
		}

        public override CollisionResult Collides(Hitbox source) {
            var graph = GetLocalGraph(source);
            if(graph == null || graph.Length < 1) {
                return CollisionResult.None;
            }
            foreach(var line in graph) {
                var result = LineIntersects(source,line);
                
                //for now, ignore inside colliding lines

                if(result.Intersection == IntersectionResult.Intersects) {
                    return new CollisionResult(line,result.ClipLine.Value);
                }
            }
            return CollisionResult.None;
        }
    }
}
