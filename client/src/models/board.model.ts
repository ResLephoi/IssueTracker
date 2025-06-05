import { List } from "./list.model";

export interface Board {
  id: string;
  title: string;
  lists: List[];
}