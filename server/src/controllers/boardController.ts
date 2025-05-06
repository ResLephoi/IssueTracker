import { Request, Response } from 'express';
import { BoardModel } from '../models/boardModel';

export class BoardController {
  async getBoards(req: Request, res: Response) {
    try {
      const boards = await BoardModel.find();
      res.status(200).json(boards);
    } catch (error) {
      res.status(500).json({ message: 'Error retrieving boards', error });
    }
  }

  async createBoard(req: Request, res: Response) {
    const newBoard = new BoardModel(req.body);
    try {
      const savedBoard = await newBoard.save();
      res.status(201).json(savedBoard);
    } catch (error) {
      res.status(400).json({ message: 'Error creating board', error });
    }
  }

  async updateBoard(req: Request, res: Response) {
    const { id } = req.params;
    try {
      const updatedBoard = await BoardModel.findByIdAndUpdate(id, req.body, { new: true });
      if (!updatedBoard) {
        return res.status(404).json({ message: 'Board not found' });
      }
      res.status(200).json(updatedBoard);
    } catch (error) {
      res.status(400).json({ message: 'Error updating board', error });
    }
  }

  async deleteBoard(req: Request, res: Response) {
    const { id } = req.params;
    try {
      const deletedBoard = await BoardModel.findByIdAndDelete(id);
      if (!deletedBoard) {
        return res.status(404).json({ message: 'Board not found' });
      }
      res.status(204).send();
    } catch (error) {
      res.status(500).json({ message: 'Error deleting board', error });
    }
  }
}