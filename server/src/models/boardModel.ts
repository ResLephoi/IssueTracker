import { Schema, model } from 'mongoose';

const cardSchema = new Schema({
  title: { type: String, required: true },
  description: { type: String, required: true },
  labels: { type: [String], default: [] }
});

const listSchema = new Schema({
  title: { type: String, required: true },
  cards: { type: [cardSchema], default: [] }
});

const boardSchema = new Schema({
  title: { type: String, required: true },
  lists: { type: [listSchema], default: [] }
});

const BoardModel = model('Board', boardSchema);

export default BoardModel;