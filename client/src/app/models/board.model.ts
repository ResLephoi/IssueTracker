export interface Card {
  id: string;
  title: string;
  description: string;
  labels: string[];
}

export interface List {
  id: string;
  title: string;
  cards: Card[];
}

export interface Board {
  id: string;
  title: string;
  lists: List[];
}