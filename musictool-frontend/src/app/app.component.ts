import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component, ElementRef, ViewChild } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, HttpClientModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'musictool-frontend';
  apiRoot = '/api'
  @ViewChild('audioPlayer')
  audioPlayer!: ElementRef<HTMLAudioElement>;

  songName = 'Sense.mp3'
  constructor(private http: HttpClient) { }

  ngAfterViewInit() {
    this.addHandlers()
  }

  /** This will be undefined if retrieved before ngAfterViewInit */
  get audioPlayerElement(): HTMLAudioElement {
    return this.audioPlayer.nativeElement
  }

  get songTitle() {
    return navigator.mediaSession.metadata?.title || 'No Title'
  }

  get songArtist() {
    return navigator.mediaSession.metadata?.artist || 'No Artist'
  }

  get songAblum() {
    return navigator.mediaSession.metadata?.album || 'No Album'
  }

  get songArtUrl() {
    return navigator.mediaSession.metadata?.artwork[0].src || ''
  }

  get mediaSessionState() {

    return {
      metadata: {
        title: navigator.mediaSession.metadata?.title || '',
        artist: navigator.mediaSession.metadata?.artist || '',
        album: navigator.mediaSession.metadata?.album || '',
        artwork: navigator.mediaSession.metadata?.artwork || []
      },
      playbackState: navigator.mediaSession.playbackState || 'none',

    };
  }

  async playAudio() {
    this.audioPlayerElement.src = `/api/download/${this.songName}`;
    await this.audioPlayerElement.play();
    await this.updateMetadata();

  }

  pauseAudio() {
    this.audioPlayerElement.pause()
  }

  async updateMetadata() {
    const track = await firstValueFrom(this.http.get<{ title: string, artist: string, album: string, artwork: MediaImage[] }>(`${this.apiRoot}/media_metadata/${this.songName}`, {}))

    console.log('Playing ' + track.title + ' track...');
    console.log(navigator.mediaSession)
    navigator.mediaSession.metadata = new MediaMetadata({
      title: track.title,
      artist: track.artist,
      album: track.album,
      artwork: track.artwork
    });
    console.log(navigator.mediaSession)

    // Media is loaded, set the duration.
    this.updatePositionState();
  }

  /* Position state (supported since Chrome 81) */

  updatePositionState() {
    if ('setPositionState' in navigator.mediaSession) {
      console.log('Updating position state...');
      navigator.mediaSession.setPositionState({
        duration: this.audioPlayerElement.duration,
        playbackRate: this.audioPlayerElement.playbackRate,
        position: this.audioPlayerElement.currentTime
      });
    }
  }

  addHandlers() {
    console.log('Adding Handlers')

    /* Previous Track & Next Track */

    navigator.mediaSession.setActionHandler('previoustrack', () => {
      console.log('> User clicked "Previous Track" icon.');
      // index = (index - 1 + playlist.length) % playlist.length;
      // playAudio();
    });

    navigator.mediaSession.setActionHandler('nexttrack', () => {
      console.log('> User clicked "Next Track" icon.');
      // index = (index + 1) % playlist.length;
      // playAudio();
    });

    this.audioPlayerElement.addEventListener('ended', () => {
      // Play automatically the next track when audio ends.
      // index = (index - 1 + playlist.length) % playlist.length;
      // playAudio();
    });

    /* Seek Backward & Seek Forward */

    let defaultSkipTime = 10; /* Time to skip in seconds by default */

    navigator.mediaSession.setActionHandler('seekbackward', (event) => {
      console.log('> User clicked "Seek Backward" icon.');
      const skipTime = event.seekOffset || defaultSkipTime;
      this.audioPlayerElement.currentTime = Math.max(this.audioPlayerElement.currentTime - skipTime, 0);
      this.updatePositionState();
    });

    navigator.mediaSession.setActionHandler('seekforward', (event) => {
      console.log('> User clicked "Seek Forward" icon.');
      const skipTime = event.seekOffset || defaultSkipTime;
      this.audioPlayerElement.currentTime = Math.min(this.audioPlayerElement.currentTime + skipTime, this.audioPlayerElement.duration);
      this.updatePositionState();
    });

    /* Play & Pause */

    navigator.mediaSession.setActionHandler('play', async () => {
      console.log('> User clicked "Play" icon.');
      await this.audioPlayerElement.play();
      // Do something more than just playing audio...
    });

    navigator.mediaSession.setActionHandler('pause', () => {
      console.log('> User clicked "Pause" icon.');
      this.audioPlayerElement.pause();
      // Do something more than just pausing audio...
    });

    this.audioPlayerElement.addEventListener('play', () => {
      navigator.mediaSession.playbackState = 'playing';
    });

    this.audioPlayerElement.addEventListener('pause', () => {
      navigator.mediaSession.playbackState = 'paused';
    });

    /* Stop (supported since Chrome 77) */

    try {
      navigator.mediaSession.setActionHandler('stop', () => {
        console.log('> User clicked "Stop" icon.');
        // TODO: Clear UI playback...
      });
    } catch (error) {
      console.log('Warning! The "stop" media session action is not supported.');
    }

    /* Seek To (supported since Chrome 78) */

    try {
      navigator.mediaSession.setActionHandler('seekto', (event) => {
        console.log('> User clicked "Seek To" icon.');
        if (!event.seekTime) return;
        if (event.fastSeek && ('fastSeek' in this.audioPlayerElement)) {
          this.audioPlayerElement.fastSeek(event.seekTime);
          return;
        }
        this.audioPlayerElement.currentTime = event.seekTime;
        this.updatePositionState();
      });
    } catch (error) {
      console.log('Warning! The "seekto" media session action is not supported.');
    }
  }
}
