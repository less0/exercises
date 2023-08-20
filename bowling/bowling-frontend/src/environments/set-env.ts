const setEnv = () => {
    const fs = require('fs');
    const writeFile = fs.writeFile;
  // Configure Angular `environment.ts` file path
    const targetPath = './src/environments/auth0.ts';
  // Load node modules
    const colors = require('colors');
    require('dotenv').config({
      path: 'src/environments/.env'
    });
  // `environment.ts` file structure
    const envConfigFile = `export const auth0Configuration = {
    domain: '${process.env["AUTH0_DOMAIN"]}',
    clientId: '${process.env["AUTH0_CLIENT_ID"]}'
  };
  `;
    console.log(colors.magenta('The file `auth0.ts` will be written with the following content: \n'));
    console.log(envConfigFile);
    writeFile(targetPath, envConfigFile, (err: any) => {
      if (err) {
        console.error(err);
        throw err;
      } else {
        console.log(colors.magenta(`Angular environment.ts file generated correctly at ${targetPath} \n`));
      }
    });
  };
  
  setEnv();
  