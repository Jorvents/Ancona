using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.CameraProjection;
using BulletSharp;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace Ancona;

public class Player
{
    public Vector2 rotation;

    public Camera3D camera;

    public Vector3 location;
    public Vector3 OLDlocation;

    public Vector3 offsetLook;
    public Vector3 offsetMove;

    public Quaternion quaternionLook;
    public Quaternion quaternionMove;

    public Vector3 velocity;

    public Model model;

    public float timeSinceLastUpdate;
    
    float speed = 20f;
    float sensitivity = 500f;

    public float amount;

    //public BoundingBox[] boxes;
    public Mesh[] meshes = new Mesh[15];

    //private Ray rayground;
    //private Ray raywall;
    
    private const int ArraySizeX = 5, ArraySizeY = 5, ArraySizeZ = 5;
    private BulletSharp.Vector3 startPosition = new BulletSharp.Vector3(0, 2, 0);

    private RigidBody body;
    private DynamicsWorld world;

    public Player(Model model)
    {
        location = new Vector3(-5.0f, 50.0f, 5.0f);
        camera.position = location;                                 // Camera position
        //camera.target = new Vector3(2.0f);                       // Camera looking at point
        camera.up = new Vector3(0.0f, 1.0f, 0.0f);           // Camera up vector (rotation towards target)
        camera.fovy = 80.0f;                                      // Camera field-of-view Y                             
        camera.projection = CAMERA_PERSPECTIVE;                  // Camera mode type
        
        rotation -= new Vector2(0, 0);

        Model map = Raylib.LoadModel("Assets/map8.obj");
        
        unsafe
        {
            for (int i = 0; i < 15; i++)
            {
                meshes[i] = map.meshes[i];
            }
        }
        
        Raylib.UnloadModel(map);

        this.model = model;

        DefaultCollisionConfiguration defaultCollisionConfiguration = new DefaultCollisionConfiguration();
        CollisionDispatcher collisionDispatcher = new CollisionDispatcher(defaultCollisionConfiguration);

        DbvtBroadphase broadphase = new DbvtBroadphase();

        world = new DiscreteDynamicsWorld(collisionDispatcher, broadphase, null, defaultCollisionConfiguration);

        world.Gravity = new BulletSharp.Vector3(0, -1,0);
        
        CreateGround();
        CreateBoxes();

        //this.boxes = boxes;

        //model.meshes[0]
        /*
        Material material = Raylib.LoadMaterialDefault();
        
        unsafe
        {
            model.materials[0] = material;
            mesh = model.meshes[0];
            model.materials[0].maps[0].texture = texture;
        }   
        */
        //mesh = Raylib.GenMeshCylinder(5f, 10f, 5);
        //texture = Raylib.LoadTexture("Assets/____SMP.png");
        //Console.WriteLine("AAAAAAAA" + model.meshCount);
    }
    private void CreateGround()
    {
        var groundShape = new BoxShape(50, 1, 50);
        //groundShape.InitializePolyhedralFeatures();
        //var groundShape = new StaticPlaneShape(Vector3.UnitY, 1);

        CollisionObject ground = new CollisionObject();
        ground.CollisionShape = groundShape;
        ground.UserObject = "Ground";
        
        world.AddCollisionObject(ground);
    }

    private void CreateBoxes()
    {
        const float mass = 1.0f;
        var colShape = new BoxShape(1);
        BulletSharp.Vector3 localInertia = colShape.CalculateLocalInertia(mass);

        var rbInfo = new RigidBodyConstructionInfo(mass, null, colShape, localInertia);

        for (int y = 0; y < ArraySizeY; y++)
        {
            for (int x = 0; x < ArraySizeX; x++)
            {
                for (int z = 0; z < ArraySizeZ; z++)
                {
                    BulletSharp.Vector3 position = startPosition + 2 * new BulletSharp.Vector3(x, y, z);

                    // make it drop from a height
                    position += new BulletSharp.Vector3(0, 80, 0);

                    // using MotionState is recommended, it provides interpolation capabilities
                    // and only synchronizes 'active' objects
                    rbInfo.MotionState = new DefaultMotionState(Matrix.Translation(position));
                    body = new RigidBody(rbInfo);

                    world.AddRigidBody(body);
                }
            }
        }

        rbInfo.Dispose();
    }

    public void Play(bool look)
    {
        if (look)
        {
            rotation.X += Raylib.GetMouseDelta().X / sensitivity;
            rotation.Y += Raylib.GetMouseDelta().Y / sensitivity;
        }

        quaternionLook = Quaternion.CreateFromYawPitchRoll(-rotation.X, rotation.Y, 0f);
        quaternionMove = Quaternion.CreateFromYawPitchRoll(-rotation.X, 0f, 0f);

        offsetLook = Vector3.Transform(new Vector3(0, 0, 1), quaternionLook) * sensitivity;
        camera.target = location + offsetLook;
        
        offsetMove = Vector3.Transform(new Vector3(0, 0, 1), quaternionMove);
        
        if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
        {
            location.X += Raylib.GetFrameTime() * speed * offsetMove.X;
            location.Z += Raylib.GetFrameTime() * speed * offsetMove.Z;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
        {
            location.X += Raylib.GetFrameTime() * speed * -offsetMove.X;
            location.Z += Raylib.GetFrameTime() * speed * -offsetMove.Z;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
        {
            location.X += Raylib.GetFrameTime() * speed * offsetMove.Z;
            location.Z += Raylib.GetFrameTime() * speed * -offsetMove.X;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
        {
            location.X += Raylib.GetFrameTime() * speed * -offsetMove.Z;
            location.Z += Raylib.GetFrameTime() * speed * offsetMove.X;
        }

        bool touchinground = false;
        bool touchingwall = false;
        
        /*
        foreach (var box in boxes)
        {
            if (Raylib.CheckCollisionBoxSphere(box, location, 2f))
            {
                touching = true;
            }
        }
        */
        
        //rayground = new Ray(location, new Vector3(0,-1,0));
        //raywall = new Ray(location, offsetMove);

        float distance = 0f;
        /*
        foreach (var mesh in meshes)
        {
            //RayCollision collisionground = Raylib.GetRayCollisionMesh(rayground, mesh, Matrix4x4.CreateTranslation(location));
            //RayCollision collisionwall = Raylib.GetRayCollisionMesh(raywall, mesh, Matrix4x4.CreateTranslation(location));
            if (collisionground.hit)
            {
                if (collisionground.distance <= 5f)
                {
                    touchinground = true;
                    distance = collisionground.distance;
                }
            }
        }
        */
        if (location.Y < 5f)
        {
            touchinground = true;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE) && touchinground)
        {
            //Console.WriteLine("Jump");
            velocity.Y = 30f;
        }
        else if (touchinground)
        {
            velocity.Y = 0;
        }
        if (!touchinground)
        {
            velocity.Y -= 48f * Raylib.GetFrameTime();
        }

        if (distance < 5 && distance != 0 && touchinground)
        {
            location.Y += 5.1f - distance;
            Console.WriteLine(distance);
        }
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_R))
        {
            location = new Vector3(-5.0f, 50.0f, 5.0f);
            velocity.Y = 0;
        }
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_T))
        {
            Matrix matrix = body.WorldTransform;
            matrix.Origin = new BulletSharp.Vector3(matrix.Origin.X, 50f, matrix.Origin.Z);
            body.WorldTransform = matrix;
            //body.LinearVelocity = new BulletSharp.Vector3();
        }
        //FREE CAM
        //FOR LATER USE
        /*
        if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
        {
            camera.position.X += Raylib.GetFrameTime() * speed * offset.X;
            camera.position.Z += Raylib.GetFrameTime() * speed * offset.Z;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
        {
            camera.position.X += Raylib.GetFrameTime() * speed * -offset.X;
            camera.position.Z += Raylib.GetFrameTime() * speed * -offset.Z;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
        {
            camera.position.X += Raylib.GetFrameTime() * speed * offset.Z;
            camera.position.Z += Raylib.GetFrameTime() * speed * -offset.X;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
        {
            camera.position.X += Raylib.GetFrameTime() * speed * -offset.Z;
            camera.position.Z += Raylib.GetFrameTime() * speed * offset.X;
        }
        
        if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE))
        {
            camera.position.Y += Raylib.GetFrameTime() * speed * sensitivity;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
        {
            camera.position.Y += Raylib.GetFrameTime() * speed * -sensitivity;
        }
        */
    }

    public void Work(float updateInterval)
    {
        //Console.WriteLine(world.CollisionObjectArray[0].InterpolationLinearVelocity);
        Console.WriteLine(body.WorldTransform.Origin);

        world.StepSimulation(updateInterval, 10);
        location.Y += velocity.Y * Raylib.GetFrameTime();
        
        camera.position = location + new Vector3(0,3,0);

        if (timeSinceLastUpdate <= updateInterval)
        {
            timeSinceLastUpdate += Raylib.GetFrameTime();
        }

        amount = timeSinceLastUpdate / updateInterval;
        
        //Console.WriteLine(amount);
    }
    public void DrawMultiplayer()
    {
        //Raylib.DrawSphere(camera.position, 5f, Color.RED);
        
        //Raylib.DrawModel(model, camera.position,0.05f,Color.WHITE);

        //amount = timeSinceLastUpdate / updateInterval;
        
        //Console.WriteLine(amount);
        //Vector3 lerped = Vector3.Lerp(OLDlocation, location, amount);
        Raylib.DrawModel(model, location,0.5f,Color.WHITE);
        
        //Raylib.DrawRay(ray, Color.LIME);
        //Raylib.DrawLine3D(ray.position, Vector3.Zero, Color.LIME);

        //Raylib.DrawModel(model, OLDlocation, 0.5f, new Color(255, 0, 0, 150));
        //Raylib.DrawModel(model, location,0.5f,new Color(0,0,255,150));
        
        //Raylib.DrawModel(model, camera.position,5f,Color.WHITE);
        //Raylib.DrawModel(model, camera.position,50f,Color.WHITE);

        //Raylib.DrawMesh(mesh, Raylib.LoadMaterialDefault(), Raylib.GetCameraMatrix(camera));
        //Raylib.DrawModel(model, camera.position,500f,Color.WHITE);
        //Raylib.DrawModel(model, camera.position,5000f,Color.WHITE);
        //Raylib.DrawModel(model, camera.position,50000f, Color.WHITE);

        //Raylib.DrawMesh(model.meshes[0], Raylib.LoadMaterialDefault(), Raylib.GetCameraMatrix(camera));
        //model.transform.

        //Raylib.DrawLine3D(camera.position, camera.target, Color.GREEN);
        /*
         Raylib.DrawText("Rotation: "+ rotation, 10, 40, 20, Color.DARKGRAY);
        Raylib.DrawText("Posistion: "+ camera.position, 10, 80, 20, Color.DARKGRAY);
        Raylib.DrawText("Mouse delta: "+Raylib.GetMouseDelta(), 10, 120, 20, Color.DARKGRAY);
        Raylib.DrawText("Target: " + camera.target, 10, 160, 20, Color.DARKGRAY);
        Raylib.DrawText("Quaternion: "+ quaternion, 10, 240, 20, Color.DARKGRAY);
        */
    }

    public void DrawLocal()
    {
        //Raylib.DrawRay(rayground, Color.LIME);
        
        Raylib.DrawSphere(vectorbulletTOSystem(body.WorldTransform.Origin), 10f ,Color.RED);
    }

    public void Tick()
    {
        timeSinceLastUpdate = 0;
    }

    public Vector3 vectorbulletTOSystem(BulletSharp.Vector3 vector3)
    {
        return new Vector3(vector3.X, vector3.Y, vector3.Z);
    }
}