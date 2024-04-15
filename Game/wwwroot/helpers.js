function scrollToElement(id) {
    const element = document.getElementById(id);
    const parent = element.parentElement;

    const ancestor = parent.parentElement;
    const elementOffset = element.offsetLeft;
    const elementWidth = element.clientWidth;
    const visibleContainerWidth = ancestor.clientWidth;

    parent.scrollTo(elementOffset - (visibleContainerWidth - elementWidth) / 2, 0);
}

const musicVolumeSettingsKey = 'settings:musicVolume';
const otherVolumeSettingsKey = 'settings:otherVolume';

function playSound(id) {
    var audio = new Audio();
    audio.autoplay = true;
    audio.src = `Sounds/${id}.ogg`;
    audio.volume = Number(localStorage[otherVolumeSettingsKey]) || 0.5;
    audio.play();
}

let activeMusic = new Audio();
let activeMusicId = null;
function onMusicVolumeUpdated(volumePercentage) {
    activeMusic.volume = volumePercentage;
}
function onOtherVolumeUpdated(volumePercentage) {
    
}
function playMusic(id) {
    if (id != activeMusicId) {
        activeMusicId = id;

        const uri = `Music/${id}.ogg`;
        activeMusic.pause();
        activeMusic.autoplay = true;
        activeMusic.loop = true;
        activeMusic.src = uri;
        activeMusic.play();
    }
    activeMusic.volume = Number(localStorage[musicVolumeSettingsKey]) || 0.5;
}
function isInPage(node) {
    return (node === document.body) ? false : document.body.contains(node);
}

const layers = [];
let nextTry = 0;
window.addEventListener("mousemove", e => {
    if (layers.length == 0) {
        --nextTry;
        if (nextTry <= 0) {
            layers.length = 0;
            layers.push(...document.querySelectorAll(".layer"));
        }
        else return;
    }

    if (layers.length == 0) {
        return;
    }
    else if (!isInPage(layers[0])) {
        nextTry = 10;
        layers.length = 0;
        return;
    }

    const middleX = window.innerWidth / 2;
    const offsetX = (e.clientX - middleX) / middleX;

    let x = 0;
    let scale = [0.125, 0.25, 0.5, 1, 2];
    for (const layer of layers) {
        ++x;
        const offsetScaleX = offsetX * scale[x] * -20;
        layer.style.left = `${offsetScaleX}px`;
    }
});

/*
 Start the autoplay of autoplayable audio elements
 when the player interacts with the page
*/
let callback;
callback = () => {
    const audio = document.getElementsByTagName("audio");
    for (const a of audio) {
        if (a.autoplay) {
            a.play();
        }
    }

    window.removeEventListener("click", callback);
};
window.addEventListener("click", callback);