using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysX;
using System.Numerics;
//namespace testPhysX.Net

class Program
{
    static void Main(string[] args)
    {
        Foundation foundation = new Foundation();
        Physics physics = new Physics(foundation);
        Scene scene = physics.CreateScene();

        Matrix4x4 mat = Matrix4x4.CreateTranslation(new Vector3(0, 0, 2));
        RigidDynamic actor = scene.Physics.CreateRigidDynamic(mat);

        Material material = scene.Physics.CreateMaterial(0.7f, 0.7f, 0.1f);

        SphereGeometry sphereGeo = new SphereGeometry(1);
        actor.CreateShape(sphereGeo, material);
        scene.AddActor(actor);

        HitFlag flag = HitFlag.Default | HitFlag.Normal;
        bool ret = scene.Raycast(Vector3.Zero, new Vector3(0, 0, 1), 100, 100, HitCallback, flag);

        Console.WriteLine(ret);

        scene.Dispose();
        physics.Dispose();
        foundation.Dispose();

        Console.WriteLine("运行结束");
        Console.ReadLine();
    }

    static bool HitCallback(RaycastHit[] array)
    {
        //Console.WriteLine("HitCallback" + flag);
        Console.WriteLine("HitCallback " + array.Length);
        return true;
    }
}

