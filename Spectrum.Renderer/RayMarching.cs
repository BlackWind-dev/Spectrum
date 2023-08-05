using System;
using System.Numerics;

namespace Spectrum.Renderer
{
	public class RayMarching
	{
		public static uint Tick = 0;

		Vector3 Vector3Normalize(Vector3 a)
		{
			float length = a.Length();
			return a / length;
		}

		Vector3 Vector3SumFloat(Vector3 a, float b)
		{
			Vector3 c = new(a.X + b, a.Y + b, a.Z + b);
			return c;
		}

		Vector3 Vector3SubtractFloat(Vector3 a, float b)
		{
			Vector3 c = new(a.X - b, a.Y - b, a.Z - b);
			return c;
		}

		float Vector3Dot(Vector3 a, Vector3 b)
		{
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}

		private float ComponentMesh(Vector3 p)
		{
			float radius = 2f;

			float angle = Tick * MathF.PI / 180;
			//float temp = p.Y;
			var cos = MathF.Cos(angle);
			var sin = MathF.Sin(angle);
			Vector3 temp = new(p.X * cos + p.Z * sin, p.Y, -p.X * sin + p.Z * cos);
			
			float displacement = MathF.Cos(5.0f * temp.X) * MathF.Cos(5.0f * temp.Y) * MathF.Cos(5.0f * temp.Z) * 0.25f;
			return temp.Length() - radius + displacement;
		}

		public Vector3 CalculateNormal(Vector3 p)
		{
			var small_step = new Vector3(0.01f, 0.0f, 0.0f);
			var temp0 = Vector3.Zero;

			temp0 = new(small_step.X, small_step.Y, small_step.Y);
			float gradient_x = ComponentMesh(p + temp0) - ComponentMesh(p - temp0);
			temp0 = new(small_step.Y, small_step.X, small_step.Y);
			float gradient_y = ComponentMesh(p + temp0) - ComponentMesh(p - temp0);
			temp0 = new(small_step.Y, small_step.Y, small_step.X);
			float gradient_z = ComponentMesh(p + temp0) - ComponentMesh(p - temp0);

			Vector3 normal = new(gradient_x, gradient_y, gradient_z);
			normal = Vector3Normalize(normal);
			return normal;
		}

		public int Render(Vector3 ro, Vector3 rd)
		{

			float total_distance_traveled = 0.0f;
			const int NUMBER_OF_STEPS = 32;
			const float MINIMUM_HIT_DISTANCE = 0.001f;
			const float MAXIMUM_TRACE_DISTANCE = 100.0f;

			for (int i = 0; i < NUMBER_OF_STEPS; i++)
			{
				Vector3 current_position = ro + rd * total_distance_traveled;

				float distance_to_closest = ComponentMesh(current_position);

				if (distance_to_closest < MINIMUM_HIT_DISTANCE)
				{
					Vector3 normal = CalculateNormal(current_position);
					Vector3 light_position = new(2.0f, 5.0f, 3.0f);
					Vector3 direction_to_light = current_position - light_position;
					direction_to_light = Vector3Normalize(direction_to_light);

					float diffuse_intensity = MathF.Max(0, Vector3Dot(normal, direction_to_light));

					return (byte)(0xFF * diffuse_intensity);
				}

				if (total_distance_traveled > MAXIMUM_TRACE_DISTANCE)
				{
					break;
				}
				total_distance_traveled += distance_to_closest;
			}
			return 0;
		}
	}
}