import { Router } from 'express';
import { getBoards, createBoard, updateBoard, deleteBoard } from '../controllers/boardController';

const router = Router();

router.get('/', getBoards);
router.post('/', createBoard);
router.put('/:id', updateBoard);
router.delete('/:id', deleteBoard);

export default router;