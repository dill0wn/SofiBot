#!/usr/bin/env osascript -l JavaScript

function run(argv) {
	console.log(`running with argv: ${JSON.stringify(argv)}`);
	const Photos = Application("Photos");

	// Missing out the () on albums causes an error
	for (const album of Photos.albums()) {
		//console.log(album.name());
	}

	for (const mediaItem of Photos.search({for:"sleeping"})) {
		logMediaItem(mediaItem);
	}
}

function logMediaItem(mediaItem) {
  console.log(`MediaItem ${mediaItem.id()}, ${mediaItem.filename()}, ${mediaItem.name()}, ${mediaItem.description()}, ${mediaItem.keywords()}`);
}