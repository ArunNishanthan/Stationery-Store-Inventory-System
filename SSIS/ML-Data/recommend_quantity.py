# -*- coding: utf-8 -*-
# Raja Sudalaimuthu Padma
from flask import Flask, jsonify, request
from flask_cors import CORS
from sklearn.externals import joblib

app = Flask(__name__)
CORS(app)

model_file_path = "./models/"


@app.route('/recommend_quantity', methods=['GET', 'POST'])
def recommend_quantity():

    req = request.get_json(force=True)
    print("recommend_quantity request received")
    print(req)

    recommend_req = req['recommend_req']

    recommend_res = []
    item_number = ""

    for record in recommend_req:

        if item_number != record["item_number"]:
            item_number = record["item_number"]
            model = joblib.load(model_file_path + item_number + ".joblib")
            print("recommendation model loaded for item_number : ", item_number)

        quantity = model.predict([[record["year"], record["month"]]])

        record["quantity"] = int(quantity[0])
        recommend_res.append(record)

    return jsonify({"recommend_res": recommend_res})


@app.route("/", methods=['GET'])
def default():
    print("Please call recommend_quantity API for quantity recommendation")
    return "<h1> Recommend Quantity API 1.0 <h1>"


if __name__ == "__main__":
    app.run(debug=False)
