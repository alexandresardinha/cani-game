using UnityEngine;

namespace Canicross.Player
{
    public static class CharacterBuilder
    {
        public static GameObject BuildDog()
        {
            GameObject dog = new GameObject("Dog");

            Transform dogTransform = dog.transform;
            dogTransform.localScale = Vector3.one;

            Color furColor = new Color(0.83f, 0.63f, 0.3f);
            Color maskColor = new Color(0.11f, 0.11f, 0.11f);
            Color bellyColor = new Color(0.9f, 0.8f, 0.6f);
            Color eyeWhite = Color.white;
            Color eyeBlack = new Color(0.1f, 0.1f, 0.1f);
            Color noseColor = new Color(0.15f, 0.1f, 0.1f);
            Color collarColor = new Color(0f, 0.75f, 1f);

            Material furMat = CreateMat(furColor, 0.1f);
            Material maskMat = CreateMat(maskColor, 0.1f);
            Material bellyMat = CreateMat(bellyColor, 0.1f);
            Material eyeMat = CreateMat(eyeWhite, 0.3f);
            Material pupilMat = CreateMat(eyeBlack, 0.3f);
            Material noseMat = CreateMat(noseColor, 0.5f);
            Material collarMat = CreateMat(collarColor, 0.2f);

            GameObject body = CreateBody(furMat, bellyMat);
            body.transform.SetParent(dogTransform);
            body.transform.localPosition = Vector3.zero;

            GameObject head = CreateHead(furMat, maskMat, eyeMat, pupilMat, noseMat);
            head.transform.SetParent(dogTransform);
            head.transform.localPosition = new Vector3(0, 0.25f, 0.35f);

            GameObject snout = CreateSnout(maskMat);
            snout.transform.SetParent(dogTransform);
            snout.transform.localPosition = new Vector3(0, 0.15f, 0.45f);

            GameObject nose = CreateNose(noseMat);
            nose.transform.SetParent(dogTransform);
            nose.transform.localPosition = new Vector3(0, 0.19f, 0.5f);

            GameObject leftEye = CreateEye(eyeMat, pupilMat, true);
            leftEye.transform.SetParent(dogTransform);
            leftEye.transform.localPosition = new Vector3(-0.08f, 0.32f, 0.42f);

            GameObject rightEye = CreateEye(eyeMat, pupilMat, false);
            rightEye.transform.SetParent(dogTransform);
            rightEye.transform.localPosition = new Vector3(0.08f, 0.32f, 0.42f);

            GameObject leftEar = CreateEar(maskMat);
            leftEar.transform.SetParent(dogTransform);
            leftEar.transform.localPosition = new Vector3(-0.1f, 0.42f, 0.22f);
            leftEar.transform.localRotation = Quaternion.Euler(15f, 0f, 15f);

            GameObject rightEar = CreateEar(maskMat);
            rightEar.transform.SetParent(dogTransform);
            rightEar.transform.localPosition = new Vector3(0.1f, 0.42f, 0.22f);
            rightEar.transform.localRotation = Quaternion.Euler(15f, 0f, -15f);

            CreateLegs(dogTransform, furMat);
            GameObject tail = CreateTail(maskMat);
            tail.transform.SetParent(dogTransform);
            tail.transform.localPosition = new Vector3(0, 0.25f, -0.25f);
            tail.transform.localRotation = Quaternion.Euler(-30f, 0f, 0f);

            GameObject collar = CreateCollar(collarMat);
            collar.transform.SetParent(dogTransform);
            collar.transform.localPosition = new Vector3(0, 0.22f, 0.2f);

            GameObject tetherAttach = new GameObject("TetherAttachPoint");
            tetherAttach.transform.SetParent(dogTransform);
            tetherAttach.transform.localPosition = new Vector3(0, 0.2f, -0.1f);

            Rigidbody rb = dog.AddComponent<Rigidbody>();
            rb.mass = 22f;
            rb.linearDamping = 1f;
            rb.angularDamping = 5f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.useGravity = true;

            DogController dogCtrl = dog.AddComponent<DogController>();
            dog.tag = "Dog";
            dog.layer = LayerMask.NameToLayer("Default");

            return dog;
        }

        public static GameObject BuildRunner()
        {
            GameObject runner = new GameObject("Runner");

            Transform runnerTransform = runner.transform;

            Color skinColor = new Color(0.7f, 0.5f, 0.35f);
            Color shirtColor = new Color(0.9f, 0.4f, 0.1f);
            Color shortsColor = new Color(0.15f, 0.15f, 0.2f);
            Color shoeColor = new Color(0.2f, 0.2f, 0.2f);

            Material skinMat = CreateMat(skinColor, 0.3f);
            Material shirtMat = CreateMat(shirtColor, 0.15f);
            Material shortsMat = CreateMat(shortsColor, 0.1f);
            Material shoeMat = CreateMat(shoeColor, 0.1f);

            GameObject torso = CreateTorso(shirtMat);
            torso.transform.SetParent(runnerTransform);
            torso.transform.localPosition = Vector3.zero;

            GameObject head = CreateRunnerHead(skinMat);
            head.transform.SetParent(runnerTransform);
            head.transform.localPosition = new Vector3(0, 0.7f, 0);

            GameObject leftArm = CreateArm(skinMat);
            leftArm.transform.SetParent(runnerTransform);
            leftArm.transform.localPosition = new Vector3(-0.2f, 0.35f, 0);

            GameObject rightArm = CreateArm(skinMat);
            rightArm.transform.SetParent(runnerTransform);
            rightArm.transform.localPosition = new Vector3(0.2f, 0.35f, 0);

            GameObject leftLeg = CreateLegRunner(shortsMat, skinMat);
            leftLeg.transform.SetParent(runnerTransform);
            leftLeg.transform.localPosition = new Vector3(-0.1f, -0.35f, 0);

            GameObject rightLeg = CreateLegRunner(shortsMat, skinMat);
            rightLeg.transform.SetParent(runnerTransform);
            rightLeg.transform.localPosition = new Vector3(0.1f, -0.35f, 0);

            GameObject leftShoe = CreateShoe(shoeMat);
            leftShoe.transform.SetParent(runnerTransform);
            leftShoe.transform.localPosition = new Vector3(-0.1f, -0.72f, 0.03f);

            GameObject rightShoe = CreateShoe(shoeMat);
            rightShoe.transform.SetParent(runnerTransform);
            rightShoe.transform.localPosition = new Vector3(0.1f, -0.72f, 0.03f);

            GameObject tetherAttach = new GameObject("TetherAttachPoint");
            tetherAttach.transform.SetParent(runnerTransform);
            tetherAttach.transform.localPosition = new Vector3(0, 0.3f, 0.2f);

            CharacterController cc = runner.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.25f;
            cc.center = new Vector3(0, 0.9f, 0);

            RunnerController runnerCtrl = runner.AddComponent<RunnerController>();
            runner.tag = "Runner";
            runner.layer = LayerMask.NameToLayer("Default");

            return runner;
        }

        #region Dog Parts

        private static GameObject CreateBody(Material furMat, Material bellyMat)
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "DogBody";
            body.transform.localScale = new Vector3(0.25f, 0.2f, 0.4f);
            body.transform.localPosition = new Vector3(0, 0.15f, 0);

            Renderer renderer = body.GetComponent<Renderer>();
            renderer.material = furMat;

            return body;
        }

        private static GameObject CreateHead(Material furMat, Material maskMat, Material eyeMat, Material pupilMat, Material noseMat)
        {
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "DogHead";
            head.transform.localScale = new Vector3(0.18f, 0.16f, 0.17f);

            head.GetComponent<Renderer>().material = maskMat;

            return head;
        }

        private static GameObject CreateSnout(Material maskMat)
        {
            GameObject snout = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            snout.name = "DogSnout";
            snout.transform.localScale = new Vector3(0.1f, 0.06f, 0.08f);

            snout.GetComponent<Renderer>().material = maskMat;

            return snout;
        }

        private static GameObject CreateNose(Material noseMat)
        {
            GameObject nose = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            nose.name = "DogNose";
            nose.transform.localScale = new Vector3(0.04f, 0.03f, 0.03f);

            nose.GetComponent<Renderer>().material = noseMat;

            return nose;
        }

        private static GameObject CreateEye(Material eyeMat, Material pupilMat, bool isLeft)
        {
            GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eye.name = isLeft ? "DogEyeL" : "DogEyeR";
            eye.transform.localScale = new Vector3(0.04f, 0.04f, 0.02f);

            eye.GetComponent<Renderer>().material = eyeMat;

            return eye;
        }

        private static GameObject CreateEar(Material maskMat)
        {
            GameObject ear = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            ear.name = "DogEar";
            ear.transform.localScale = new Vector3(0.05f, 0.08f, 0.03f);

            ear.GetComponent<Renderer>().material = maskMat;

            return ear;
        }

        private static void CreateLegs(Transform parent, Material furMat)
        {
            Vector3[] legPositions = new Vector3[]
            {
                new Vector3(-0.1f, -0.05f, 0.15f),
                new Vector3(0.1f, -0.05f, 0.15f),
                new Vector3(-0.1f, -0.05f, -0.15f),
                new Vector3(0.1f, -0.05f, -0.15f)
            };

            string[] legNames = { "LegFL", "LegFR", "LegBL", "LegBR" };

            for (int i = 0; i < 4; i++)
            {
                GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                leg.name = "Dog_" + legNames[i];
                leg.transform.SetParent(parent);
                leg.transform.localPosition = legPositions[i];
                leg.transform.localScale = new Vector3(0.05f, 0.12f, 0.05f);
                leg.GetComponent<Renderer>().material = furMat;

                GameObject paw = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                paw.name = "Dog_Paw_" + legNames[i];
                paw.transform.SetParent(parent);
                paw.transform.localPosition = legPositions[i] + new Vector3(0, -0.14f, 0);
                paw.transform.localScale = new Vector3(0.06f, 0.03f, 0.07f);
                paw.GetComponent<Renderer>().material = CreateMat(new Color(0.12f, 0.12f, 0.12f), 0.05f);
            }
        }

        private static GameObject CreateTail(Material maskMat)
        {
            GameObject tail = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            tail.name = "DogTail";
            tail.transform.localScale = new Vector3(0.03f, 0.12f, 0.03f);
            tail.GetComponent<Renderer>().material = maskMat;
            return tail;
        }

        private static GameObject CreateCollar(Material collarMat)
        {
            GameObject collar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            collar.name = "DogCollar";
            collar.transform.localScale = new Vector3(0.18f, 0.02f, 0.18f);
            collar.GetComponent<Renderer>().material = collarMat;
            return collar;
        }

        #endregion

        #region Runner Parts

        private static GameObject CreateTorso(Material shirtMat)
        {
            GameObject torso = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            torso.name = "RunnerTorso";
            torso.transform.localScale = new Vector3(0.3f, 0.25f, 0.18f);
            torso.GetComponent<Renderer>().material = shirtMat;

            return torso;
        }

        private static GameObject CreateRunnerHead(Material skinMat)
        {
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "RunnerHead";
            head.transform.localScale = new Vector3(0.15f, 0.17f, 0.15f);
            head.GetComponent<Renderer>().material = skinMat;

            return head;
        }

        private static GameObject CreateArm(Material skinMat)
        {
            GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            arm.name = "RunnerArm";
            arm.transform.localScale = new Vector3(0.06f, 0.2f, 0.06f);
            arm.GetComponent<Renderer>().material = skinMat;

            return arm;
        }

        private static GameObject CreateLegRunner(Material shortsMat, Material skinMat)
        {
            GameObject upperLeg = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            upperLeg.name = "RunnerLeg";
            upperLeg.transform.localScale = new Vector3(0.08f, 0.18f, 0.08f);
            upperLeg.GetComponent<Renderer>().material = shortsMat;

            return upperLeg;
        }

        private static GameObject CreateShoe(Material shoeMat)
        {
            GameObject shoe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shoe.name = "RunnerShoe";
            shoe.transform.localScale = new Vector3(0.08f, 0.04f, 0.14f);
            shoe.GetComponent<Renderer>().material = shoeMat;

            return shoe;
        }

        #endregion

        private static Material CreateMat(Color color, float smoothness)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = color;
            mat.SetFloat("_Glossiness", smoothness);
            return mat;
        }
    }
}