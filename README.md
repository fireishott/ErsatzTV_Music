# ErsatzTV_Music

Custom fork of ErsatzTV with enhanced API support for manual collections and session tracking.

## 🚀 Features Added

- **Manual Collection API**: Add/remove items to manual collections via API
- **Session Tracking API**: Get now-playing information including artist, track, filename, time remaining
- **Port**: Runs on **8410** (instead of default 8409)

## 📡 API Endpoints

### Manual Collections
- `GET /api/collections/manual` - List all manual collections
- `GET /api/collections/manual/{id}` - Get specific collection
- `POST /api/collections/manual/new` - Create collection
- `PUT /api/collections/manual/update/{id}` - Update collection
- `DELETE /api/collections/manual/delete/{id}` - Delete collection
- `POST /api/collections/manual/{id}/items` - Add items to collection
- `DELETE /api/collections/manual/{id}/items` - Remove items from collection

### Session Tracking
- `GET /api/sessions/now/{channelId}` - Get now-playing for channel
- `GET /api/sessions/now` - Get now-playing for all channels
- `GET /api/sessions/active` - List active sessions

## 🐳 Docker

```bash
# Pull the image
docker pull fireishott/ersatztv-music:latest

# Run container (port 8410)
docker run -d \
  --name ersatztv-music \
  -p 8410:8410 \
  -v /path/to/config:/config \
  -v /path/to/media:/media \
  -e PUID=1000 \
  -e PGID=1000 \
  -e TZ=America/New_York \
  fireishott/ersatztv-music:latest




# ErsatzTV

ErsatzTV lets you transform your media library into a personalized, live TV experience - complete with EPG, channel scheduling, and seamless streaming to all your devices. Rediscover your content, your way.

[![contact](https://img.shields.io/badge/contact_us-510b80?style=for-the-badge)](https://ersatztv.org/contact)
[![features](https://img.shields.io/badge/vote_on_features-darkgreen?style=for-the-badge)](https://features.ersatztv.org/)
[![community](https://img.shields.io/badge/join_the_community-blue?style=for-the-badge)](https://discuss.ersatztv.org)

![epg-example](https://ersatztv.org/images/home/epg-example.png)

## How It Works

1. **Install ErsatzTV**: Download and set up the server on your system.
2. **Add Your Media**: Connect your media libraries and collections.
3. **Create Channels**: Design and schedule your own live channels.
4. **Stream Anywhere**: Watch on any device with IPTV and EPG support.

## Key Features

- **Custom channels**: Create and schedule your own live TV channels.
- **IPTV & EPG**: Stream with IPTV and Electronic Program Guide support.
- **Hardware Transcoding**: High-performance streaming with hardware acceleration (NVENC, QSV, VAAPI, AMF, VideoToolbox)
- **Media Server Integration**: Connect Plex, Jellyfin, Emby and more.
- **Music & Subtitles**: Mix music videos and enjoy subtitle support.
- **Open Source**: Free, open, and community-driven project.

## Documentation

Documentation is available at [ersatztv.org](https://ersatztv.org/docs/).

## License

This project is inspired by [pseudotv-plex](https://github.com/DEFENDORe/pseudotv) and
the [dizquetv](https://github.com/vexorian/dizquetv) fork and is released under the [zlib license](LICENSE).
