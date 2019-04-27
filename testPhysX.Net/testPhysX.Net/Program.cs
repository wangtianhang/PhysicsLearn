using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysX;
using System.Numerics;
//namespace testPhysX.Net
//using UnityEngine;

class Program
{
    static void Main(string[] args)
    {
        UnityEngine.Application.Init();

        Foundation foundation = new Foundation();
        Physics physics = new Physics(foundation);
        Scene scene = physics.CreateScene();
        //scene.Gravity = new Vector3(0, 0, 0);
        //scene.Gravity = new Vector3(0, -9.8f, 0);

        Matrix4x4 mat = Matrix4x4.CreateTranslation(new Vector3(0, 0, 2));
        RigidDynamic actor = scene.Physics.CreateRigidDynamic(mat);

        Material material = scene.Physics.CreateMaterial(0.7f, 0.7f, 0.1f);

        SphereGeometry sphereGeo = new SphereGeometry(1);
        actor.CreateShape(sphereGeo, material);
        //actor.Mass = 1;
        scene.AddActor(actor);

        for(int i = 0; i < 100; ++i)
        {
            scene.Simulate(0.1f);
            scene.FetchResults(true);
            HitFlag flag = HitFlag.Default | HitFlag.Normal;
            bool ret = scene.Raycast(Vector3.Zero, new Vector3(0, 0, 1), 100, 100, HitCallback, flag);
            Console.WriteLine("Raycast " + ret);
        }

        scene.Dispose();
        physics.Dispose();
        foundation.Dispose();

        Console.WriteLine("运行结束");
        Console.ReadLine();
    }

    static bool HitCallback(RaycastHit[] array)
    {
        //Console.WriteLine("HitCallback" + flag);
        if(array.Length > 0)
        {
            Console.WriteLine("HitCallback " + array[0].Position + " " + array[0].Actor.GlobalPose.Translation);
        }
        return true;
    }
}

