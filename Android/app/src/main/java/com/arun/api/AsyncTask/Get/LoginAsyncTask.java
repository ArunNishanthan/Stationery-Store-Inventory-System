package com.arun.api.AsyncTask.Get;

import android.os.AsyncTask;

import com.arun.api.Model.User;
import com.google.gson.Gson;

import org.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

public class LoginAsyncTask extends AsyncTask<String, Void,JSONObject> {
    public Icallback icallback;

    public LoginAsyncTask( Icallback icallback){
        this.icallback=icallback;
    }

    @Override
    protected JSONObject doInBackground(String... strings) {
        JSONObject jsonObj = null;
        StringBuilder response = new StringBuilder();
        try {
            URL url = new URL("http://10.0.2.2:1256/api/Login/CheckUser?email="+strings[0]+"&password="+strings[1]);
            HttpURLConnection conn = (HttpURLConnection) url.openConnection();
            conn.setRequestMethod("GET");
// receive response
            try {
                InputStream inputStream = new BufferedInputStream(conn.getInputStream());
                BufferedReader r = new BufferedReader(new InputStreamReader(inputStream));
                for (String line; (line = r.readLine()) != null; ) {
                    response.append(line).append('\n');
                }
                jsonObj = new JSONObject(response.toString());
                jsonObj.put("context", "set");
            } catch (Exception e) {
                e.printStackTrace();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return jsonObj;
    }

    @Override
    protected void onPostExecute(JSONObject jsonObj) {
        if (jsonObj == null)
            icallback.checkUserLogin(null);
        try {
            String context = (String) jsonObj.get("context");
            if (context.compareTo("set") == 0)
            {

                Gson gson = new Gson();

                User user = gson.fromJson(jsonObj.toString(),User.class);
                /*user.setUserName((String)jsonObj.get("UserName"));
                user.setEmail((String)jsonObj.get("Email"));
                int role=(int)jsonObj.get("Role");
                user.setRole(role);*/
                icallback.checkUserLogin(user);
            }
        }catch (Exception e){

        }

    }

public interface Icallback{
    void checkUserLogin(User user);
}
}
