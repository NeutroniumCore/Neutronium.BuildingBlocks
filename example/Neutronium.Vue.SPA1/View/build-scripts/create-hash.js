const { getSourceHash, getBuildHash } = require('./hash-builder');
const { filePath } = require('./config');

const fs = require('fs');

async function saveHash() {
  let hashSource;
  let hashBuild;
  try {
    hashSource = await getSourceHash();
    hashBuild = await getBuildHash();
  }
  catch(exception) {
    console.log("Problem hash file not updated");
    return;
  }
  const hash = {
    hashSource,
    hashBuild
  }
  fs.writeFile(filePath, JSON.stringify(hash), function (err) {
    if (err) {
      console.log("Problem hash file not updated");
      return;
    }
    console.log("hash file updated");
  });
}

saveHash();