package com.arun.api.AsyncTask.Get;

import android.os.AsyncTask;

import com.arun.api.Model.User;
import com.arun.api.ViewModel.AssignDeptRepViewModel;
import com.google.gson.Gson;

import org.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

public class AssignRepAsyncTask extends AsyncTask<String, Void, JSONObject> {
    public Icallback icallback;

    public AssignRepAsyncTask(Icallback icallback) {
        this.icallback = icallback;
    }

    @Override
    protected JSONObject doInBackground(String... strings) {
        String id =strings[0];
        JSONObject jsonObj = null;
        StringBuilder response = new StringBuilder();
        try {
            URL url = new URL("http://10.0.2.2:1256/api/Employee/AssignDeptRepForm?empId=" + id);
            HttpURLConnection conn = (HttpURLConnection) url.openConnection();
            conn.setRequestMethod("GET");
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
            icallback.getAssignRepModel(null);
        try {
            String context = (String) jsonObj.get("context");
            if (context.compareTo("set") == 0) {
                Gson gson = new Gson();
                AssignDeptRepViewModel assignDeptRepViewModel=gson.fromJson(jsonObj.toString(),AssignDeptRepViewModel.class);
                icallback.getAssignRepModel(assignDeptRepViewModel);
            }
        } catch (Exception e) {
System.out.println(e.getMessage());
        }

    }

    public interface Icallback {
        void getAssignRepModel(AssignDeptRepViewModel assignDeptRepViewModel);
    }
}
