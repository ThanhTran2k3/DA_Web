// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
/*const $ = document.querySelector.bind(document);
const $$ = document.querySelectorAll.bind(document);

//const progress = 
const player = $('.player')
const playBtn = $('.btn-toggle-play')
const anotherPlay = $('.playNow')
const audio = $('#audio')
const progress = $('#progress')




const app = {
    isPlaying: false,

    handleEvents: function () {
        const _this = this
        playBtn.onclick = function () {
            if (_this.isPlaying) {
                _this.isPlaying = false
                audio.pause()
                player.classList.remove('playing')
            } else {
                _this.isPlaying = true
                audio.play()
                player.classList.add('playing')
            }
        }

        anotherPlay.onclick = function () {
            if (_this.isPlaying) {
                _this.isPlaying = false
                audio.pause()
                player.classList.remove('playing')
            } else {
                _this.isPlaying = true
                audio.play()
                player.classList.add('playing')
            }
        }

        audio.ontimeupdate = function () {
            if (audio.duration) {
                const progressPercent = Math.floor(audio.currentTime / audio.duration * 100)
                progress.value = progressPercent
            }
        }

        progress.onchange = function (e) {
            const seekTime = audio.duration / 100 * e.target.value
            audio.currentTime = seekTime
        }

        window.addEventListener('beforeunload', function () {
            localStorage.setItem('audioPlaying', _this.isPlaying);
            localStorage.setItem('audioPosition', audio.currentTime);
        });
    },
    start: function () {
        const storedAudioPlaying = localStorage.getItem('audioPlaying');
        if (storedAudioPlaying === 'true') {
            this.isPlaying = true;
            player.classList.add('playing');
            audio.play();
        } else {
            this.isPlaying = false;
            player.classList.remove('playing');
        }

        const storedAudioPosition = parseFloat(localStorage.getItem('audioPosition'));
        if (!isNaN(storedAudioPosition)) {
            audio.currentTime = storedAudioPosition;
        }
        this.handleEvents()
    },
}
app.start()*/


/*    const $ = document.querySelector.bind(document);
    const $$ = document.querySelectorAll.bind(document);

    //const progress =
    const player = $('.player')
    const playBtn = $('.btn-toggle-play')
    const audio = $('#audio')
    const progress = $('#progress')
    const app = {
        isPlaying: false,

    checkSongPath: function (newSongPath) {
             const oldSongPath = localStorage.getItem('currentSongPath');
    if (oldSongPath !== null) {
                 if (oldSongPath !== newSongPath) {
        localStorage.setItem('currentSongPath', newSongPath);
    localStorage.setItem('currentTime', 0);
    localStorage.setItem('isPlaying', false);
    if ('@currentController' !== 'Home') {
        localStorage.setItem('isPlaying', true);
                     }
                 }
    else {
                     if ('@currentController' === 'Home') {
        localStorage.setItem('isPlaying', false);
    this.loadState()
                     }
                     
                 }
             }
    else {
        localStorage.setItem('currentSongPath', newSongPath);
             }
         },

    loadState: function () {
             const savedState = localStorage.getItem('isPlaying');
    const savedTime = localStorage.getItem('currentTime');
    if (savedState !== null) {
        this.isPlaying = savedState === 'true';
    if (this.isPlaying) {
        audio.play();
    player.classList.add('playing');
    audio.currentTime = savedTime;
                 }
    else {
        audio.pause()
                     player.classList.remove('playing')
                 }
             }
         },
    handleEvents: function () {
             const _this = this
    playBtn.onclick = function () {
                 if (_this.isPlaying) {
        _this.isPlaying = false
                     audio.pause()
    player.classList.remove('playing')
    localStorage.setItem('isPlaying', _this.isPlaying);
                 } else {
        _this.isPlaying = true
                     audio.play()
    player.classList.add('playing')
    localStorage.setItem('isPlaying', _this.isPlaying);
                 }
             }



    audio.ontimeupdate = function () {
                 if (audio.duration) {
                     const progressPercent = Math.floor(audio.currentTime / audio.duration * 100)
    localStorage.setItem('currentTime', audio.currentTime);
    progress.value = progressPercent
                     if (audio.currentTime >= audio.duration) {
        player.classList.remove('playing');
    _this.isPlaying = false;
    localStorage.setItem('isPlaying', _this.isPlaying);
                     }
                 }
                
             }

    progress.onchange = function (e) {
                 const seekTime = audio.duration / 100 * e.target.value
    audio.currentTime = seekTime
    localStorage.setItem('currentTime', audio.currentTime);
             }
         },
    start: function () {
             const newSongPath = '@randomBaiHat.FilePath';

    this.checkSongPath(newSongPath);
    this.loadState()
    this.handleEvents()
         },
     }
    app.start()*/


