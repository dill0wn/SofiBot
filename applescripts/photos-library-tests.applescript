#!/usr/bin/env osascript -l JavaScript

function run(argv) {
	const app = Application.currentApplication();
	app.includeStandardAdditions = true;
	//console.log(`running with argv: ${JSON.stringify(argv)}`);

	const Photos = Application("Photos");
	
	// https://www.galvanist.com/posts/2020-03-28-jxa_notes/#supported-filter-operators
	const sofi = Photos.albums.sofi;
	const photos = sofi.mediaItems.whose({
		
		_not: [
			{ filename: {_endsWith: '.MOV'}},
		]
	});
	const randomIndex = Math.floor(Math.random() * photos.length);
	
	const thephoto = photos.at(randomIndex);
	//console.log(`random ${randomIndex}, ${thephoto.filename()}`);

	// const albums = Photos.albums.whose({
	// 	name: { _contains	: 'sofi'}
	// });

	//for (const mediaItem of Photos.search({for:"sofi,sleeping"})) {
	//	logMediaItem(mediaItem);
	//}
	
	//const tmpPath = app.pathTo('temporary items', {from: 'classic domain'});
	const tmpPath = Path("/tmp/sofi");
	//console.log(`tmp thing ${tmpPath}`);
	
	const result = Photos.export([thephoto], {to: tmpPath, useOriginals: false})

	const dest = Path(tmpPath + "/" + thephoto.filename());
	return dest;
}

function logMediaItem(mediaItem) {
  console.log(`MediaItem ${mediaItem.id()}, ${mediaItem.filename()}, ${mediaItem.name()}, ${mediaItem.description()}, ${mediaItem.keywords()}`);
}