const { getSourceHash, getBuildHash } = require('./hash-builder');
const { filePath} = require('./config');
const fs = require('fs');

let data
try {
  data = fs.readFileSync(filePath, { encoding: 'utf-8' });
}
catch (error) {
  console.log("problem reading file");
  process.exit(1);
}

const {hashSource, hashBuild} = JSON.parse(data);

async function checkHashes(){
  try{
    const build = await getBuildHash();
    if (build!==hashBuild) {
      console.log("build");
      process.exit(1);
    }
    const source = await getSourceHash();
    if (source!==hashSource) {
      console.log("source");
      process.exit(1);
    }
    console.log("same hashes doing nothing");
    process.exit(0);
  }
  catch {
    process.exit(1);
  }
}

checkHashes();