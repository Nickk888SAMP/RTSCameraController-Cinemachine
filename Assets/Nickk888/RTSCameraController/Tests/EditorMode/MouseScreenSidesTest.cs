using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class MouseScreenSidesTest
{
    GameObject gameObject;
    RTSCameraTargetController script;
    Canvas canvas;

    [SetUp]
    public void SetUp()
    {
        gameObject = new GameObject();
        
        script = gameObject.AddComponent<RTSCameraTargetController>();
        script.ScreenSidesZoneSize = 50;

        canvas = gameObject.AddComponent<Canvas>();
        script.RTSCanvasRectTransform = canvas.GetComponent<RectTransform>();
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(gameObject);
    }

    #region Unit Tests

    [Test]
    public void Center()
    {
        script.GetMouseScreenSideXY(new Vector2(Screen.width / 2, Screen.height / 2), out int width, out int height);
        Assert.AreEqual(0, width, "Width");
        Assert.AreEqual(0, height, "height");
    }

    [Test]
    public void Top()
    {
        for (int i = 0; i <= script.ScreenSidesZoneSize; i++)
        {
            script.GetMouseScreenSideXY(new Vector2(Screen.width / 2, i), out int width, out int height);
            Assert.AreEqual(0, width, "Width");
            Assert.AreEqual(-1, height, "height");
        }
    }

    [Test]
    public void TopLeft()
    {
        for (int i = 0; i <= script.ScreenSidesZoneSize; i++)
        {
            script.GetMouseScreenSideXY(Vector2.one * i, out int width, out int height);
            Assert.AreEqual(-1, width, "Width");
            Assert.AreEqual(-1, height, "height");
        }
    }

    [Test]
    public void TopRight()
    {
        for (int i = 0; i <= script.ScreenSidesZoneSize; i++)
        {
            script.GetMouseScreenSideXY(new Vector2(Screen.width - i, i), out int width, out int height);
            Assert.AreEqual(1, width, "Width");
            Assert.AreEqual(-1, height, "height");
        }
    }

    [Test]
    public void BottomLeft()
    {
        for (int i = 0; i <= script.ScreenSidesZoneSize; i++)
        {
            script.GetMouseScreenSideXY(new Vector2(i, Screen.height - i), out int width, out int height);
            Assert.AreEqual(-1, width, "Width");
            Assert.AreEqual(1, height, "height");
        }
    }

    [Test]
    public void BottomRight()
    {
        for (int i = 0; i <= script.ScreenSidesZoneSize; i++)
        {
            script.GetMouseScreenSideXY(new Vector2(Screen.width - i, Screen.height - i), out int width, out int height);
            Assert.AreEqual(1, width, "Width");
            Assert.AreEqual(1, height, "height");
        }
    }

    [Test]
    public void Bottom()
    {
        for (int i = 0; i <= script.ScreenSidesZoneSize; i++)
        {
            script.GetMouseScreenSideXY(new Vector2(Screen.width / 2, Screen.height - i), out int width, out int height);
            Assert.AreEqual(0, width, "Width");
            Assert.AreEqual(1, height, "height");
        }
    }

    [Test]
    public void Left()
    {
        for (int i = 0; i <= script.ScreenSidesZoneSize; i++)
        {
            script.GetMouseScreenSideXY(new Vector2(i, Screen.height / 2), out int width, out int height);
            Assert.AreEqual(-1, width, "Width");
            Assert.AreEqual(0, height, "height");
        }
    }

    [Test]
    public void Right()
    {
        for (int i = 0; i <= script.ScreenSidesZoneSize; i++)
        {
            script.GetMouseScreenSideXY(new Vector2(Screen.width - i, Screen.height / 2), out int width, out int height);
            Assert.AreEqual(1, width, "Width");
            Assert.AreEqual(0, height, "height");
        }
    }

    #endregion

}
