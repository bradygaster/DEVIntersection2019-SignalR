var plane, controls, camera, scene, renderer;
var cameraCube, sceneCube;
var textureEquirec, textureCube, textureSphere;
var cubeMesh, planeMesh;
var planeMaterial;

init();
animate();

function init() {

    // CAMERAS

    camera = new THREE.PerspectiveCamera(70, window.innerWidth / window.innerHeight, 1, 100000);
    camera.position.set(0, 0, 1000);
    cameraCube = new THREE.PerspectiveCamera(70, window.innerWidth / window.innerHeight, 1, 100000);

    controls = new THREE.OrbitControls(camera);
    controls.minDistance = 500;
    controls.maxDistance = 2500;

    // SCENE

    scene = new THREE.Scene();
    sceneCube = new THREE.Scene();

    // Lights

    var ambient = new THREE.AmbientLight(0xffffff);
    scene.add(ambient);

    // Textures

    var r = "/media/skybox/";
    var urls = [r + "px.jpg", r + "nx.jpg",
    r + "py.jpg", r + "ny.jpg",
    r + "pz.jpg", r + "nz.jpg"];

    textureCube = new THREE.CubeTextureLoader().load(urls);
    textureCube.format = THREE.RGBFormat;
    textureCube.mapping = THREE.CubeReflectionMapping;
    textureCube.encoding = THREE.sRGBEncoding;

    var textureLoader = new THREE.TextureLoader();

    textureSphere = textureLoader.load("/media/metal.jpg");
    textureSphere.mapping = THREE.SphericalReflectionMapping;
    textureSphere.encoding = THREE.sRGBEncoding;

    // Materials

    var cubeShader = THREE.ShaderLib["cube"];
    var cubeMaterial = new THREE.ShaderMaterial({
        fragmentShader: cubeShader.fragmentShader,
        vertexShader: cubeShader.vertexShader,
        uniforms: cubeShader.uniforms,
        depthWrite: false,
        side: THREE.BackSide
    });

    cubeMaterial.uniforms["tCube"].value = textureCube;
    Object.defineProperty(cubeMaterial, 'map', {
        get: function () {
            return this.uniforms.tCube.value;
        }
    });

    // Skybox

    cubeMesh = new THREE.Mesh(new THREE.BoxBufferGeometry(100, 100, 100), cubeMaterial);
    sceneCube.add(cubeMesh);

    plane = new THREE.BoxBufferGeometry(800, 800, 10);
    planeMaterial = new THREE.MeshLambertMaterial({ envMap: textureCube });
    planeMesh = new THREE.Mesh(plane, planeMaterial);

    scene.add(planeMesh);

    renderer = new THREE.WebGLRenderer();
    renderer.autoClear = false;
    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(window.innerWidth, window.innerHeight);
    document.body.appendChild(renderer.domElement);

    renderer.gammaOutput = true;

    window.addEventListener('resize', onWindowResize, false);
}

function onWindowResize() {

    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();

    cameraCube.aspect = window.innerWidth / window.innerHeight;
    cameraCube.updateProjectionMatrix();

    renderer.setSize(window.innerWidth, window.innerHeight);
}

function animate() {
    requestAnimationFrame(animate);
    render();
}

function render() {
    camera.lookAt(scene.position);
    cameraCube.rotation.copy(camera.rotation);

    renderer.render(sceneCube, cameraCube);
    renderer.render(scene, camera);
}

animate();