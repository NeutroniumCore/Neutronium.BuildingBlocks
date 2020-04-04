const { hashElement } = require('folder-hash');
const { fileName } = require('./config');

const optionsSource = {
  folders: { exclude: ['.*', 'node_modules', 'test_coverage', 'tests', 'data', 'build-scripts', 'dist'] },
  files: { exclude: ['README.md'] }
};

const optionsDist = {
  files: { exclude: [fileName] }
};

function hashBuilder(root, options){
  return async function getSourceHash() {
    try {
      const hash = await hashElement(root, options);
      return hash.hash;
    }
    catch (error) {
      console.error('hashing failed:', error);
      throw error;
    }
  }
}

exports.getSourceHash = hashBuilder('.', optionsSource);
exports.getBuildHash = hashBuilder('./dist', optionsDist);